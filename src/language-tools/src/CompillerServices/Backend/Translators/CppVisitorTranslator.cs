using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CompillerServices.Exceptions;
using CompillerServices.IO;
using Microsoft.Extensions.Localization;
using SlangGrammar;

namespace CompillerServices.Backend.Translators
{
    public class CppVisitorTranslator : SlangBaseVisitor<object>, ITranslator
    {
        #region Standard types

        private static readonly IDictionary<string, string> SystemTypes = new Dictionary<string, string>
        {
            { "bool", "bool" },
            { "int", "int64_t" },
            { "real", "double" }
        };

        #endregion

        #region System functions

        private static readonly IDictionary<string, string> SystemFunctions = new Dictionary<string, string>
        {
            { "Abs", "abs" },
            { "FAbs", "fabs" },
            { "Pow", "pow" },
            { "Log", "log10" },
            { "Ln", "log" },
            { "Exp", "exp" },
            { "Sqrt", "sqrt" }
        };

        #endregion

        private readonly IStringLocalizer<CppVisitorTranslator> _localizer;
        private readonly string _headerFileName;
        private readonly SourceContainer _sourceContainer;
        private readonly IndentedTextWriter _headerWriter;
        private readonly IndentedTextWriter _sourceWriter;

        private string _moduleName;

        // Должно быть false изначально
        private bool _shouldWriteHeader;

        public CppVisitorTranslator(IStringLocalizer<CppVisitorTranslator> localizer, string headerFileName,
            TextWriter headerWriter, TextWriter sourceWriter, SourceContainer sourceContainer)
        {
            _localizer = localizer;
            _headerFileName = headerFileName;
            _sourceContainer = sourceContainer;
            _headerWriter = new IndentedTextWriter(headerWriter);
            _sourceWriter = new IndentedTextWriter(sourceWriter);
        }

        public void Dispose()
        {
            _headerWriter?.InnerWriter?.Dispose();
            _sourceWriter?.InnerWriter?.Dispose();
            _headerWriter?.Dispose();
            _sourceWriter?.Dispose();
        }

        public async Task Translate(SlangParser parser)
        {
            await Task.Run(() => { Visit(parser.start()); });

            await _headerWriter.FlushAsync();
            await _sourceWriter.FlushAsync();
        }

        #region Start

        public override object VisitStart(SlangParser.StartContext context)
        {
            _headerWriter.WriteLine($"#ifndef {_headerFileName.Replace('.', '_')}");
            _headerWriter.WriteLine($"#define {_headerFileName.Replace('.', '_')}");
            _headerWriter.WriteLine();

            base.VisitStart(context);

            _headerWriter.WriteLine();
            _headerWriter.WriteLine("#endif");

            return null;
        }

        #endregion

        #region Imports

        public override object VisitModuleImports(SlangParser.ModuleImportsContext context)
        {
            ToggleOnlyHeader();

            WriteLine("#include <iostream>");
            WriteLine("#include <vector>");
            WriteLine("#include <functional>");
            _sourceWriter.WriteLine($"#include \"{_headerFileName}\"");

            base.VisitModuleImports(context);
            
            WriteLine();
            ToggleOnlyHeader();

            return null;
        }

        public override object VisitModuleImport(SlangParser.ModuleImportContext context)
        {
            SlangModule importingModule = _sourceContainer[context.Id().GetText()];
            string includingFile;

            if (importingModule.IsSystem)
            {
                includingFile = $"System/{context.Id().GetText()}";
            }
            else
            {
                includingFile = context.Id().GetText();
            }

            WriteLine($"#include \"{includingFile}.h\"");
            
            return null;
        }

        #endregion

        #region Types

        public override object VisitSimpleType(SlangParser.SimpleTypeContext context)
        {
            string simpleType = TranslateType(context.SimpleType().GetText());
            Write(simpleType);

            return null;
        }

        public override object VisitFuncType(SlangParser.FuncTypeContext context)
        {
            Write("std::function<");
            Visit(context.type());
            Visit(context.routineArgList());
            Write(">");

            return null;
        }

        public override object VisitProcType(SlangParser.ProcTypeContext context)
        {
            Write("std::function<void");
            Visit(context.routineArgList());
            Write(">");

            return null;
        }

        public override object VisitRoutineArgList(SlangParser.RoutineArgListContext context)
        {
            Write("(");

            SlangParser.RoutineArgContext[] args = context.routineArg();
            SlangParser.RoutineArgContext firstArg = args.FirstOrDefault();

            if (firstArg != null)
            {
                Visit(firstArg);

                foreach (SlangParser.RoutineArgContext arg in args.Skip(1))
                {
                    Write(", ");
                    Visit(arg);
                }
            }

            Write(")");

            return null;
        }

        public override object VisitRoutineArg(SlangParser.RoutineArgContext context)
        {
            Visit(context.type());

            string modifier = context.ArgPassModifier().GetText();

            if (modifier == Constants.ArgModifiers.Ref)
            {
                Write(" &");
            }

            return null;
        }

        public override object VisitArrayType(SlangParser.ArrayTypeContext context)
        {
            WriteVectorBegin(context.arrayDimention().Length);
            Visit(context.scalarType());
            WriteVectorEnd(context.arrayDimention().Length);

            return null;
        }

        #endregion

        #region Module

        public override object VisitModule(SlangParser.ModuleContext context)
        {
            _moduleName = context.Id().GetText();

            ToggleOnlyHeader();

            WriteLine($"namespace {_moduleName}\n{{");
            _sourceWriter.Indent++;
            _headerWriter.Indent++;

            ToggleOnlyHeader();

            Visit(context.moduleDeclare());
            Visit(context.moduleEntry());

            ToggleOnlyHeader();

            _sourceWriter.Indent--;
            _headerWriter.Indent--;

            WriteLine("}");

            ToggleOnlyHeader();

            return null;
        }

        public override object VisitModuleEntry(SlangParser.ModuleEntryContext context)
        {
            _headerWriter.WriteLine($"void start_{_moduleName}();");

            _sourceWriter.WriteLine($"void start_{_moduleName}()");
            Visit(context.statementSequence());

            return null;
        }

        #endregion

        #region Declare

        public override object VisitFuncDeclare(SlangParser.FuncDeclareContext context)
        {
            ToggleOnlyHeader();

            Visit(context.type());
            Write(" ");

            Write(context.Id().GetText());
            Visit(context.routineDeclareArgList());

            ToggleOnlyHeader();

            Visit(context.statementSequence());

            return null;
        }

        public override object VisitProcDeclare(SlangParser.ProcDeclareContext context)
        {
            ToggleOnlyHeader();

            Write("void ");
            Write(context.Id().GetText());
            Visit(context.routineDeclareArgList());

            ToggleOnlyHeader();

            Visit(context.statementSequence());

            return null;
        }

        public override object VisitRoutineDeclareArgList(SlangParser.RoutineDeclareArgListContext context)
        {
            Write("(");

            SlangParser.RoutineDeclareArgContext[] args = context.routineDeclareArg();
            SlangParser.RoutineDeclareArgContext firstArg = args.FirstOrDefault();

            if (firstArg != null)
            {
                Visit(firstArg);

                foreach (SlangParser.RoutineDeclareArgContext arg in args.Skip(1))
                {
                    Write(", ");
                    Visit(arg);
                }
            }

            Write(")");

            if (_shouldWriteHeader)
            {
                _headerWriter.WriteLine(";");
            }

            _sourceWriter.WriteLine();

            return null;
        }

        public override object VisitRoutineDeclareArg(SlangParser.RoutineDeclareArgContext context)
        {
            Visit(context.type());
            Write(" ");

            string modifier = context.ArgPassModifier().GetText();

            if (modifier == Constants.ArgModifiers.Ref)
            {
                Write("&");
            }

            Write(context.Id().GetText());

            return null;
        }

        #endregion

        #region Statements

        public override object VisitStatementSequence(SlangParser.StatementSequenceContext context)
        {
            WriteLine("{");
            _sourceWriter.Indent++;

            base.VisitStatementSequence(context);

            _sourceWriter.Indent--;
            WriteLine("}");
            WriteLine();

            return null;
        }

        public override object VisitSingleStatement(SlangParser.SingleStatementContext context)
        {
            base.VisitSingleStatement(context);
            WriteLine(";");

            return null;
        }

        public override object VisitScalarDeclare(SlangParser.ScalarDeclareContext context)
        {
            Visit(context.scalarType());

            Write(" ");
            Write(context.Id().GetText());

            if (context.mathExp() != null)
            {
                Write(" = ");
                Visit(context.mathExp());
            }

            if (context.boolOr() != null)
            {
                Write(" = ");
                Visit(context.boolOr());
            }

            return null;
        }

        public override object VisitConstDeclare(SlangParser.ConstDeclareContext context)
        {
            Write("const ");
            Visit(context.simpleType());
            Write(" ");
            Write(context.Id().GetText());

            if (context.mathExp() != null)
            {
                Write(" = ");
                Visit(context.mathExp());
            }

            if (context.boolOr() != null)
            {
                Write(" = ");
                Visit(context.boolOr());
            }

            return null;
        }

        public override object VisitArrayDeclare(SlangParser.ArrayDeclareContext context)
        {
            int dimentionCount = context.arrayDeclareType().arrayDeclareDimention().Length;
            SlangParser.ScalarTypeContext type = context.arrayDeclareType().scalarType();

            WriteVectorBegin(dimentionCount);
            Visit(type);
            WriteVectorEnd(dimentionCount);

            Write(" ");
            Write(context.Id().GetText());

            IEnumerable<SlangParser.MathExpContext> dimentionSizes =
                context.arrayDeclareType().arrayDeclareDimention().Select(x => x.mathExp());

            int n = dimentionCount - 1;

            foreach (SlangParser.MathExpContext dimentionSize in dimentionSizes)
            {
                Write("(");
                Visit(dimentionSize);

                if (n > 0)
                {
                    Write(", ");

                    WriteVectorBegin(n);
                    Visit(type);
                    WriteVectorEnd(n);
                }

                n--;
            }

            for (int i = 0; i < dimentionCount; i++)
            {
                Write(")");
            }

            return null;
        }

        public override object VisitSingleAssign(SlangParser.SingleAssignContext context)
        {
            ITerminalNode id = context.Id();

            Write(id.GetText());
            Write(" = ");

            base.VisitSingleAssign(context);
            return null;
        }

        public override object VisitArrayAssign(SlangParser.ArrayAssignContext context)
        {
            Visit(context.arrayElement());
            Write(" = ");

            Visit(context.GetChild(2));

            return null;
        }

        public override object VisitArrayElement(SlangParser.ArrayElementContext context)
        {
            string id = context.Id().GetText();
            Write(id);

            foreach (SlangParser.ArrayDeclareDimentionContext dimention in context.arrayDeclareDimention())
            {
                Write(".at(");
                Visit(dimention.mathExp());
                Write(")");
            }

            return null;
        }

        public override object VisitArrayLength(SlangParser.ArrayLengthContext context)
        {
            string id = context.Id().GetText();
            int dimentionIndex = int.Parse(context.IntValue().GetText());

            Write($"{id}");

            for (int i = 0; i < dimentionIndex; i++)
            {
                Write("[0]");
            }

            Write(".size()");

            return null;
        }

        public override object VisitReturn(SlangParser.ReturnContext context)
        {
            Write("return ");
            base.VisitReturn(context);

            return null;
        }

        public override object VisitInput(SlangParser.InputContext context)
        {
            ITerminalNode id = context.Id();
            Write($"std::cin >> {id.GetText()}");

            return null;
        }

        public override object VisitOutput(SlangParser.OutputContext context)
        {
            Write("std::cout");

            foreach (SlangParser.OutputOperandContext operand in context.outputOperand())
            {
                Write(" << ");
                Visit(operand);
                Write(" << ' '");
            }

            WriteLine(";");
            Write("std::cout << std::endl");

            return null;
        }

        public override object VisitOutputOperand(SlangParser.OutputOperandContext context)
        {
            if (context.exp() != null)
            {
                Visit(context.exp());
                return null;
            }

            Write(context.StringLiteral().GetText());

            return base.VisitOutputOperand(context);
        }

        public override object VisitCall(SlangParser.CallContext context)
        {
            Visit(context.id());

            Write("(");
            Visit(context.callArgList());
            Write(")");

            return null;
        }

        public override object VisitCallArgList(SlangParser.CallArgListContext context)
        {
            SlangParser.CallArgContext[] args = context.callArg();

            if (args.Length <= 0)
            {
                return null;
            }

            Visit(args[0]);

            if (args.Length <= 1)
            {
                return null;
            }

            for (int i = 1; i < args.Length; i++)
            {
                Write(", ");
                Visit(args[i]);
            }

            return null;
        }

        public override object VisitIfSingle(SlangParser.IfSingleContext context)
        {
            Write("if (");
            Visit(context.boolOr());
            WriteLine(")");
            Visit(context.statementSequence());

            return null;
        }

        public override object VisitIfElse(SlangParser.IfElseContext context)
        {
            Write("if (");
            Visit(context.boolOr());
            WriteLine(")");
            Visit(context.statementSequence(0));
            WriteLine("else");
            Visit(context.statementSequence(1));

            return null;
        }

        public override object VisitWhileLoop(SlangParser.WhileLoopContext context)
        {
            Write("while (");
            Visit(context.boolOr());
            WriteLine(")");
            Visit(context.statementSequence());

            return null;
        }

        public override object VisitDoWhileLoop(SlangParser.DoWhileLoopContext context)
        {
            WriteLine("do");
            WriteLine("{");
            _sourceWriter.Indent++;
            _headerWriter.Indent++;

            base.VisitStatementSequence(context.statementSequence());

            _sourceWriter.Indent--;
            _headerWriter.Indent--;
            Write("} while (");
            Visit(context.boolOr());
            WriteLine(");");

            return null;
        }

        #endregion

        #region Expressions

        public override object VisitMathExpSum(SlangParser.MathExpSumContext context)
        {
            Visit(context.mathTerm());
            Write(" + ");
            Visit(context.mathExp());

            return null;
        }

        public override object VisitMathExpSub(SlangParser.MathExpSubContext context)
        {
            Visit(context.mathTerm());
            Write(" - ");
            Visit(context.mathExp());

            return null;
        }

        public override object VisitMathTermMul(SlangParser.MathTermMulContext context)
        {
            Visit(context.mathFactor());
            Write(" * ");
            Visit(context.mathTerm());

            return null;
        }

        public override object VisitMathTermDiv(SlangParser.MathTermDivContext context)
        {
            Visit(context.mathFactor());
            Write(" / ");
            Visit(context.mathTerm());

            return null;
        }

        public override object VisitMathTermMod(SlangParser.MathTermModContext context)
        {
            Visit(context.mathFactor());
            Write(" % ");
            Visit(context.mathTerm());

            return null;
        }

        public override object VisitMathFactorBrackets(SlangParser.MathFactorBracketsContext context)
        {
            Write("(");
            Visit(context.mathExp());
            Write(")");

            return null;
        }

        public override object VisitMathFactorUnaryPlus(SlangParser.MathFactorUnaryPlusContext context)
        {
            Write("+");
            base.VisitMathFactorUnaryPlus(context);

            return null;
        }

        public override object VisitMathFactorUnaryMinus(SlangParser.MathFactorUnaryMinusContext context)
        {
            Write("-");
            base.VisitMathFactorUnaryMinus(context);

            return null;
        }

        public override object VisitLogicOr(SlangParser.LogicOrContext context)
        {
            Visit(context.boolAnd());
            Write(" || ");
            Visit(context.boolOr());

            return null;
        }

        public override object VisitLogicAnd(SlangParser.LogicAndContext context)
        {
            Visit(context.boolEquality());
            Write(" && ");
            Visit(context.boolAnd());

            return null;
        }

        public override object VisitBoolEqual(SlangParser.BoolEqualContext context)
        {
            Visit(context.boolInequality());
            Write(" == ");
            Visit(context.boolEquality());

            return null;
        }

        public override object VisitBoolNotEqual(SlangParser.BoolNotEqualContext context)
        {
            Visit(context.boolInequality());
            Write(" != ");
            Visit(context.boolEquality());

            return null;
        }

        public override object VisitMathEqual(SlangParser.MathEqualContext context)
        {
            Visit(context.mathExp(0));
            Write(" == ");
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitMathNotEqual(SlangParser.MathNotEqualContext context)
        {
            Visit(context.mathExp(0));
            Write(" != ");
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitBigger(SlangParser.BiggerContext context)
        {
            Visit(context.mathExp(0));
            Write(" > ");
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitLesser(SlangParser.LesserContext context)
        {
            Visit(context.mathExp(0));
            Write(" < ");
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitBiggerOrEqual(SlangParser.BiggerOrEqualContext context)
        {
            Visit(context.mathExp(0));
            Write(" >= ");
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitLesserOrEqual(SlangParser.LesserOrEqualContext context)
        {
            Visit(context.mathExp(0));
            Write(" <= ");
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitNot(SlangParser.NotContext context)
        {
            Write("!");
            base.VisitNot(context);

            return null;
        }

        public override object VisitBoolAtomBrackets(SlangParser.BoolAtomBracketsContext context)
        {
            Write("(");
            base.VisitBoolAtomBrackets(context);
            Write(")");

            return null;
        }

        public override object VisitBoolAtomBracketsNot(SlangParser.BoolAtomBracketsNotContext context)
        {
            Write("!");
            Write("(");
            base.VisitBoolAtomBracketsNot(context);
            Write(")");

            return null;
        }

        public override object VisitExpAtom(SlangParser.ExpAtomContext context)
        {
            if (context.call() == null && context.arrayElement() == null && context.arrayLength() == null)
            {
                Write(context.GetChild(0).GetText());
                return null;
            }

            base.VisitExpAtom(context);
            return null;
        }

        public override object VisitId(SlangParser.IdContext context)
        {
            ITerminalNode[] ids = context.Id();

            ITerminalNode module = null;
            ITerminalNode id;

            if (ids.Length > 1)
            {
                module = ids[0];
                id = ids[1];   
            }
            else
            {
                id = ids[0];
            }

            string translated = TranslateOtherModuleId(module, id);
            Write(translated);

            return null;
        }

        public override object VisitRaw(SlangParser.RawContext context)
        {
            WriteLine();
            ICharStream characterStream = context.any().Start.InputStream;

            IToken a = context.any().Start;
            IToken b = context.any().Stop;

            Interval interval = new Interval(a.StartIndex, b.StopIndex);
            
            Write(characterStream.GetText(interval));
            WriteLine();
            return null;
        }

        #endregion

        #region Private members

        private void Write(string text)
        {
            if (_shouldWriteHeader)
            {
                _headerWriter.Write(text);
            }

            _sourceWriter.Write(text);
        }

        private void WriteLine(string text = "")
        {
            if (_shouldWriteHeader)
            {
                _headerWriter.WriteLine(text);
            }

            _sourceWriter.WriteLine(text);
        }

        private static string TranslateOtherModuleId(IParseTree module, ITerminalNode id)
        {
            if (module == null)
            {
                return $"{id.GetText()}";
            }

            string moduleName = module.GetText();
            return $"{moduleName}::{id}";
        }

        private static string TranslateType(string slangType)
        {
            return SystemTypes.TryGetValue(slangType, out string cppType) ? cppType : slangType;
        }

        private void WriteVectorBegin(int dimention)
        {
            for (int i = 0; i < dimention; i++)
            {
                Write("std::vector<");
            }
        }

        private void WriteVectorEnd(int dimention)
        {
            for (int i = 0; i < dimention; i++)
            {
                Write(">");
            }
        }

        private void ToggleOnlyHeader()
        {
            _shouldWriteHeader = !_shouldWriteHeader;
        }

        #endregion
    }
}