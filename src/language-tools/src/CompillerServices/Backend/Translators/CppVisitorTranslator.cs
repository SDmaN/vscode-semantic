using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using SlangGrammar;

namespace CompillerServices.Backend.Translators
{
    public class CppVisitorTranslator : SlangBaseVisitor<object>, ITranslator
    {
        #region Standard library modules

        private static readonly IDictionary<string, string> ModuleStandardIncludes = new Dictionary<string, string>
        {
            { "Math", "cmath" }
        };

        #endregion

        #region Standard types

        private static readonly IDictionary<string, string> StandardTypes = new Dictionary<string, string>
        {
            { "bool", "bool" },
            { "int", "int" },
            { "real", "float" }
        };

        #endregion

        private readonly string _headerFileName;
        private readonly IndentedTextWriter _headerWriter;
        private readonly IndentedTextWriter _sourceWriter;

        private string _moduleName;

        // Должно быть false изначально
        private bool _shouldWriteHeader;

        public CppVisitorTranslator(string headerFileName, TextWriter headerWriter, TextWriter sourceWriter)
        {
            _headerFileName = headerFileName.Replace('.', '_').ToUpper();
            _headerWriter = new IndentedTextWriter(headerWriter);
            _sourceWriter = new IndentedTextWriter(sourceWriter);
        }

        public void Dispose()
        {
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
            _headerWriter.WriteLine($"#ifndef {_headerFileName}");
            _headerWriter.WriteLine($"#define {_headerFileName}");
            _headerWriter.WriteLine();

            base.VisitStart(context);

            _headerWriter.WriteLine();
            _headerWriter.WriteLine("#endif");

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

            if (modifier == "ref")
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

        #region Imports

        public override object VisitModuleImports(SlangParser.ModuleImportsContext context)
        {
            ToggleOnlyHeader();

            WriteLine("#include <iostream>");
            WriteLine("#include <vector>");
            WriteLine("#include <functional>");

            foreach (ITerminalNode module in context.Id())
            {
                WriteLine($"#include \"{module.GetText()}.h\"");
                WriteLine($"using namespace {module.GetText()};");
            }

            WriteLine();
            ToggleOnlyHeader();

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

            if (modifier == "ref")
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

            foreach (SlangParser.StatementContext statement in context.statement())
            {
                Visit(statement);
            }

            _sourceWriter.Indent--;
            WriteLine("}");
            WriteLine();

            return null;
        }

        public override object VisitSimpleDeclare(SlangParser.SimpleDeclareContext context)
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

            WriteLine(";");

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

            WriteLine(";");

            return null;
        }

        public override object VisitSingleAssign(SlangParser.SingleAssignContext context)
        {
            ITerminalNode id = context.Id();

            Write(id.GetText());
            Write(" = ");

            base.VisitSingleAssign(context);

            if (context.assign() == null)
            {
                WriteLine(";");
            }

            return null;
        }

        public override object VisitArrayAssign(SlangParser.ArrayAssignContext context)
        {
            Visit(context.arrayElement());
            Write(" = ");

            Visit(context.GetChild(2));

            if (context.assign() == null)
            {
                WriteLine(";");
            }

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
            WriteLine(";");
            return null;
        }

        public override object VisitInput(SlangParser.InputContext context)
        {
            ITerminalNode id = context.Id();
            WriteLine($"std::cin >> {id.GetText()};");

            return null;
        }

        public override object VisitOutput(SlangParser.OutputContext context)
        {
            Write("std::cout << ");
            base.VisitOutput(context);
            WriteLine(" << std::endl;");

            return null;
        }

        public override object VisitCall(SlangParser.CallContext context)
        {
            ITerminalNode[] ids = context.Id();

            ITerminalNode moduleName = null;
            ITerminalNode functionName;

            if (ids.Length == 2)
            {
                moduleName = ids[0];
                functionName = ids[1];
            }
            else
            {
                functionName = ids[0];
            }

            if (moduleName != null)
            {
                Write($"{moduleName.GetText()}::");
            }

            Write($"{functionName.GetText()}(");
            base.VisitCall(context);
            WriteLine(");");

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

        public override object VisitMathAtom(SlangParser.MathAtomContext context)
        {
            var id = context.Id();
            var i = context.IntValue();
            var r = context.RealValue();
            var c = context.call();
            var ae = context.arrayElement();
            var al = context.arrayLength();

            if (context.call() == null && context.arrayElement() == null && context.arrayLength() == null)
            {
                Write(context.GetChild(0).GetText());
                return null;
            }

            base.VisitMathAtom(context);

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

        public override object VisitBoolAtom(SlangParser.BoolAtomContext context)
        {
            if (context.call() == null && context.arrayElement() == null)
            {
                Write(context.GetChild(0).GetText());
            }

            return base.VisitBoolAtom(context);
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

        private static string TranslateType(string slangType)
        {
            return StandardTypes.TryGetValue(slangType, out string cppType) ? cppType : slangType;
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