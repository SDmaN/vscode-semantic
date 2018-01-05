using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CompillerServices.Backend.Writers;
using SlangGrammar;

namespace CompillerServices.Backend
{
    internal class TranslatorVisitor : SlangBaseVisitor<object>, IDisposable
    {
        /*private readonly ISourceWriter _sourceWriter;
        private string _currentModule;

        public TranslatorVisitor(ISourceWriter sourceWriter)
        {
            _sourceWriter = sourceWriter;
        }

        public void Dispose()
        {
            _sourceWriter?.Dispose();
        }

        public override object VisitStart(SlangParser.StartContext context)
        {
            _sourceWriter.WriteStart();
            object result = base.VisitStart(context);
            _sourceWriter.WriteEnd();

            return result;
        }

        public override object VisitModuleImports(SlangParser.ModuleImportsContext context)
        {
            _sourceWriter.WriteImportBegin();

            foreach (ITerminalNode module in context.Id())
            {
                _sourceWriter.WriteImport(module.GetText());
            }

            _sourceWriter.WriteImportEnd();

            return base.VisitModuleImports(context);
        }

        public override object VisitModule(SlangParser.ModuleContext context)
        {
            _currentModule = context.Id().GetText();
            _sourceWriter.WriteModuleBegin(_currentModule);

            object result = base.VisitModule(context);

            _sourceWriter.WriteModuleEnd();

            return result;
        }

        public override object VisitFuncDeclare(SlangParser.FuncDeclareContext context)
        {
            string modifier = context.AccessModifier().GetText();
            string name = context.Id().GetText();

            _sourceWriter.WriteFunctionDeclareBegin(modifier, name);

            Visit(context.type());
            Visit(context.routineDeclareArgList());

            _sourceWriter.WriteFunctionDeclareEnd(modifier, name);
            
            return null;
        }

        public override object VisitSimpleType(SlangParser.SimpleTypeContext context)
        {
            _sourceWriter.WriteSimpleType(context.SimpleType().GetText());
            return base.VisitSimpleType(context);
        }

        public override object VisitFuncType(SlangParser.FuncTypeContext context)
        {
            _sourceWriter.WriteFunctionTypeBegin();
            Visit(context.routineArgList());
            Visit(context.type());
            _sourceWriter.WriteFunctionTypeEnd();

            return null;
        }

        public override object VisitProcType(SlangParser.ProcTypeContext context)
        {
            _sourceWriter.WriteProcedureTypeBegin();
            Visit(context.routineArgList());
            _sourceWriter.WriteProcedureTypeEnd();

            return null;
        }

        public override object VisitRoutineArgList(SlangParser.RoutineArgListContext context)
        {
            _sourceWriter.WriteRoutineArgListBegin();

            bool isFirst = true;

            foreach (SlangParser.RoutineArgContext arg in context.routineArg())
            {
                Visit(arg);

                if (!isFirst)
                {
                    _sourceWriter.WriteRoutineArgDelimeter();
                }

                isFirst = false;
            }

            _sourceWriter.WriteRoutineArgListEnd();

            return null;
        }

        public override object VisitRoutineArg(SlangParser.RoutineArgContext context)
        {
            return base.VisitRoutineArg(context);
        }

        public override object VisitFunc(SlangParser.FuncContext context)
        {
            string modifier = context.ModuleAccessModifier().GetText();
            string type = GetRuleTypeString(context.arrayOrSimpleType());
            string name = context.Id().GetText();
            IEnumerable<SubprogramArgument> arguments = CreateArguments(context.argList());

            _sourceWriter.WriteFunction(modifier, type, name, arguments);

            return Visit(context.statementBlock());
        }

        public override object VisitProc(SlangParser.ProcContext context)
        {
            string modifier = context.ModuleAccessModifier().GetText();
            string name = context.Id().GetText();
            IEnumerable<SubprogramArgument> arguments = CreateArguments(context.argList());

            _sourceWriter.WriteProcedure(modifier, name, arguments);

            return Visit(context.statementBlock());
        }

        private IEnumerable<SubprogramArgument> CreateArguments(SlangParser.ArgListContext context)
        {
            SlangParser.ArrayOrSimpleTypeContext[] argTypes = context.arrayOrSimpleType();
            ITerminalNode[] argNames = context.Id();
            ITerminalNode[] passModifiers = context.ArgPassModifier();

            IList<SubprogramArgument> arguments = new List<SubprogramArgument>(argTypes.Length);

            for (int i = 0; i < argTypes.Length; i++)
            {
                string modifier = passModifiers[i].GetText();
                string argType = GetRuleTypeString(argTypes[i]);
                string argName = argNames[i].GetText();

                arguments.Add(new SubprogramArgument(modifier, argType, argName));
            }

            return arguments;
        }

        public override object VisitStatementBlock(SlangParser.StatementBlockContext context)
        {
            _sourceWriter.WriteBlockBegin();
            object result = base.VisitStatementBlock(context);
            _sourceWriter.WriteBlockEnd();

            return result;
        }

        public override object VisitStatement(SlangParser.StatementContext context)
        {
            object result = base.VisitStatement(context);

            StatementType statementType;

            if (context.@if() == null && context.whileLoop() == null && context.doWhileLoop() == null)
            {
                statementType = StatementType.SingleStatement;
            }
            else
            {
                statementType = StatementType.BlockStatement;
            }

            _sourceWriter.WriteStatementEnd(statementType);

            return result;
        }

        public override object VisitDeclare(SlangParser.DeclareContext context)
        {
            string type = GetRuleTypeString(context.arrayOrSimpleType());

            _sourceWriter.WriteSimpleType(type);

            ITerminalNode id = context.Id();
            _sourceWriter.WriteIdentifier(id.GetText());

            TryVisitDeclareRightPart(context.mathExp());
            TryVisitDeclareRightPart(context.boolOr());
            TryVisitDeclareRightPart(context.arrayDeclare());

            return null;
        }

        private void TryVisitDeclareRightPart(IParseTree context)
        {
            if (context == null)
            {
                return;
            }

            _sourceWriter.WriteAssign();
            Visit(context);
        }

        public override object VisitArrayType(SlangParser.ArrayTypeContext context)
        {
            ITerminalNode type = context.Type();
            return _sourceWriter.GetArrayType(type.GetText(), context.ArrayTypeBrackets().Length);
        }

        public override object VisitArrayDeclare(SlangParser.ArrayDeclareContext context)
        {
            ITerminalNode type = context.Type();

            for (int i = 0; i < context.mathExp().Length; i++)
            {
                _sourceWriter.WriteArrayDimention(type.GetText(), i, context.mathExp().Length);
                Visit(context.mathExp(i));
            }

            _sourceWriter.WriteArrayEnd(context.mathExp().Length);

            return null;
        }

        public override object VisitSingleAssign(SlangParser.SingleAssignContext context)
        {
            ITerminalNode id = context.Id();
            _sourceWriter.WriteIdentifier(id.GetText());
            _sourceWriter.WriteAssign();

            return base.VisitSingleAssign(context);
        }

        public override object VisitArrayAssign(SlangParser.ArrayAssignContext context)
        {
            Visit(context.arrayElement());
            _sourceWriter.WriteAssign();

            ParserRuleContext rightContext = context.GetRuleContext<ParserRuleContext>(1);
            Visit(rightContext);

            return null;
        }

        public override object VisitArrayElement(SlangParser.ArrayElementContext context)
        {
            ITerminalNode id = context.Id();
            _sourceWriter.WriteIdentifier(id.GetText());

            foreach (SlangParser.MathExpContext m in context.mathExp())
            {
                _sourceWriter.WriteArrayElementBegin();
                Visit(m);
                _sourceWriter.WriteArrayElementEnd();
            }

            return null;
        }

        public override object VisitInput(SlangParser.InputContext context)
        {
            ITerminalNode id = context.Id();
            _sourceWriter.WriteInput(id.GetText());

            return base.VisitInput(context);
        }

        public override object VisitOutput(SlangParser.OutputContext context)
        {
            _sourceWriter.WriteOutput();
            return base.VisitOutput(context);
        }

        public override object VisitReturn(SlangParser.ReturnContext context)
        {
            _sourceWriter.WriteReturn();
            return base.VisitReturn(context);
        }

        public override object VisitMathExpSum(SlangParser.MathExpSumContext context)
        {
            Visit(context.mathTerm());
            _sourceWriter.WriteSum();
            Visit(context.mathExp());

            return null;
        }

        public override object VisitMathExpSub(SlangParser.MathExpSubContext context)
        {
            Visit(context.mathTerm());
            _sourceWriter.WriteSubstraction();
            Visit(context.mathExp());

            return null;
        }

        public override object VisitMathTermMul(SlangParser.MathTermMulContext context)
        {
            Visit(context.mathFactor());
            _sourceWriter.WriteMultiply();
            Visit(context.mathTerm());

            return null;
        }

        public override object VisitMathTermDiv(SlangParser.MathTermDivContext context)
        {
            Visit(context.mathFactor());
            _sourceWriter.WriteDivision();
            Visit(context.mathTerm());

            return null;
        }

        public override object VisitMathTermMod(SlangParser.MathTermModContext context)
        {
            Visit(context.mathFactor());
            _sourceWriter.WriteMod();
            Visit(context.mathTerm());

            return null;
        }

        public override object VisitMathFactorBrackets(SlangParser.MathFactorBracketsContext context)
        {
            _sourceWriter.WriteBraceBegin();
            object result = Visit(context.mathExp());
            _sourceWriter.WriteBraceEnd();

            return result;
        }

        public override object VisitMathFactorUnaryPlus(SlangParser.MathFactorUnaryPlusContext context)
        {
            _sourceWriter.WritePlus();
            return base.VisitMathFactorUnaryPlus(context);
        }

        public override object VisitMathFactorUnaryMinus(SlangParser.MathFactorUnaryMinusContext context)
        {
            _sourceWriter.WriteMinus();
            return base.VisitMathFactorUnaryMinus(context);
        }

        public override object VisitMathAtom(SlangParser.MathAtomContext context)
        {
            if (context.call() == null && context.arrayElement() == null)
            {
                _sourceWriter.WriteRaw(context.GetChild(0).GetText());
            }

            return base.VisitMathAtom(context);
        }

        public override object VisitLogicOr(SlangParser.LogicOrContext context)
        {
            Visit(context.boolAnd());
            _sourceWriter.WriteLogicOr();
            Visit(context.boolOr());

            return null;
        }

        public override object VisitLogicAnd(SlangParser.LogicAndContext context)
        {
            Visit(context.boolEquality());
            _sourceWriter.WriteLogicAnd();
            Visit(context.boolAnd());

            return null;
        }

        public override object VisitBoolEqual(SlangParser.BoolEqualContext context)
        {
            Visit(context.boolInequality());
            _sourceWriter.WriteEquality();
            Visit(context.boolEquality());

            return null;
        }

        public override object VisitBoolNotEqual(SlangParser.BoolNotEqualContext context)
        {
            Visit(context.boolInequality());
            _sourceWriter.WriteInequality();
            Visit(context.boolEquality());

            return null;
        }

        public override object VisitMathEqual(SlangParser.MathEqualContext context)
        {
            Visit(context.mathExp(0));
            _sourceWriter.WriteEquality();
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitMathNotEqual(SlangParser.MathNotEqualContext context)
        {
            Visit(context.mathExp(0));
            _sourceWriter.WriteEquality();
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitBigger(SlangParser.BiggerContext context)
        {
            Visit(context.mathExp(0));
            _sourceWriter.WriteBigger();
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitLesser(SlangParser.LesserContext context)
        {
            Visit(context.mathExp(0));
            _sourceWriter.WriteLesser();
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitBiggerOrEqual(SlangParser.BiggerOrEqualContext context)
        {
            Visit(context.mathExp(0));
            _sourceWriter.WriteBiggerOrEqual();
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitLesserOrEqual(SlangParser.LesserOrEqualContext context)
        {
            Visit(context.mathExp(0));
            _sourceWriter.WriteLesserOrEqual();
            Visit(context.mathExp(1));

            return null;
        }

        public override object VisitNot(SlangParser.NotContext context)
        {
            _sourceWriter.WriteNot();
            return base.VisitNot(context);
        }

        public override object VisitBoolAtomBrackets(SlangParser.BoolAtomBracketsContext context)
        {
            _sourceWriter.WriteBraceBegin();
            object result = base.VisitBoolAtomBrackets(context);
            _sourceWriter.WriteBraceEnd();

            return result;
        }

        public override object VisitBoolAtomBracketsNot(SlangParser.BoolAtomBracketsNotContext context)
        {
            _sourceWriter.WriteNot();
            _sourceWriter.WriteBraceBegin();
            object result = base.VisitBoolAtomBracketsNot(context);
            _sourceWriter.WriteBraceEnd();

            return result;
        }

        public override object VisitBoolAtom(SlangParser.BoolAtomContext context)
        {
            if (context.call() == null && context.arrayElement() == null)
            {
                _sourceWriter.WriteRaw(context.GetChild(0).GetText());
            }

            return base.VisitBoolAtom(context);
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

            _sourceWriter.WriteFunctionCallBegin(functionName.GetText(), moduleName?.GetText());
            object result = base.VisitCall(context);
            _sourceWriter.WriteFunctionCallEnd();

            return result;
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
                _sourceWriter.WriteCallArgSeparator();
                Visit(args[i]);
            }

            return null;
        }

        public override object VisitIfSingle(SlangParser.IfSingleContext context)
        {
            _sourceWriter.WriteIfBegin();
            Visit(context.boolOr());
            _sourceWriter.WriteIfEnd();
            Visit(context.statementBlock());

            return null;
        }

        public override object VisitIfElse(SlangParser.IfElseContext context)
        {
            _sourceWriter.WriteIfBegin();
            Visit(context.boolOr());
            _sourceWriter.WriteIfEnd();
            Visit(context.statementBlock(0));
            _sourceWriter.WriteElse();
            Visit(context.statementBlock(1));

            return null;
        }

        public override object VisitWhileLoop(SlangParser.WhileLoopContext context)
        {
            _sourceWriter.WriteWhileBegin();
            Visit(context.boolOr());
            _sourceWriter.WriteWhileEnd();
            Visit(context.statementBlock());

            return null;
        }

        public override object VisitDoWhileLoop(SlangParser.DoWhileLoopContext context)
        {
            _sourceWriter.WriteDo();
            Visit(context.statementBlock());
            _sourceWriter.WriteDoWhileBegin();
            Visit(context.boolOr());
            _sourceWriter.WriteDoWhileEnd();

            return null;
        }

        private string GetRuleTypeString(SlangParser.ArrayOrSimpleTypeContext context)
        {
            return context.arrayType() != null ? (string) Visit(context.arrayType()) : context.Type().ToString();
        }*/
        public void Dispose()
        {
        }
    }
}