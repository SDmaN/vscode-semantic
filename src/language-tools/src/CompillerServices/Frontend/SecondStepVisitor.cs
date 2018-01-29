using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CompillerServices.Frontend.NameTables;
using CompillerServices.Frontend.NameTables.Types;
using CompillerServices.IO;
using Microsoft.Extensions.Localization;
using SlangGrammar;

namespace CompillerServices.Frontend
{
    public class SecondStepVisitor : BaseStepVisitor
    {
        private static readonly IEnumerable<string> Keywords = new HashSet<string>
        {
            "import",
            "module",
            "fun",
            "proc",
            "end",
            "int",
            "real",
            "bool",
            "const",

            "class"
        };

        private readonly ModuleNameTableRow _currentModuleRow;
        private readonly IStringLocalizer<SecondStepVisitor> _localizer;
        private readonly INameTableContainer _nameTableContainer;
        private RoutineNameTableRow _currentRoutineRow;

        public SecondStepVisitor(IStringLocalizer<SecondStepVisitor> localizer, INameTableContainer nameTableContainer,
            SlangModule slangModule)
            : base(slangModule)
        {
            _localizer = localizer;
            _nameTableContainer = nameTableContainer;
            _currentModuleRow = _nameTableContainer.ModuleNameTable.FindModule(slangModule.ModuleName);
        }

        public override object VisitModuleImports(SlangParser.ModuleImportsContext context)
        {
            ICollection<string> alreadyImported = new HashSet<string>();

            foreach (ITerminalNode importingModule in context.Id())
            {
                ThrowIfCorrespondingToKeyword(importingModule);

                string moduleName = importingModule.GetText();

                if (!_nameTableContainer.ModuleNameTable.Contains(moduleName))
                {
                    ThrowCompillerException(_localizer["Module '{0}' is not declared.", moduleName],
                        importingModule.Symbol);
                }

                if (alreadyImported.Contains(moduleName))
                {
                    ThrowCompillerException(_localizer["Module '{0}' already imported.", moduleName],
                        importingModule.Symbol);
                }

                if (moduleName == _currentModuleRow.ModuleName)
                {
                    ThrowCompillerException(_localizer["The module can't import itself."], importingModule.Symbol);
                }

                alreadyImported.Add(moduleName);
            }

            return null;
        }

        public override object VisitModule(SlangParser.ModuleContext context)
        {
            ITerminalNode id = context.Id();
            ThrowIfCorrespondingToKeyword(id);

            Visit(context.moduleDeclare());
            Visit(context.moduleEntry());

            return null;
        }

        public override object VisitFuncDeclare(SlangParser.FuncDeclareContext context)
        {
            ITerminalNode id = context.Id();

            _currentRoutineRow =
                _currentModuleRow.FindFunctionByPosition(id.GetText(), id.Symbol.Line, id.Symbol.Column);

            StatementResult statementResult = (StatementResult) Visit(context.statementSequence());

            if (!statementResult.ReturnsValue)
            {
                ThrowCompillerException(_localizer["Not all code path returns a value."], id.Symbol);
            }

            return null;
        }

        public override object VisitProcDeclare(SlangParser.ProcDeclareContext context)
        {
            ITerminalNode id = context.Id();

            _currentRoutineRow =
                _currentModuleRow.FindProcedureByPosition(id.GetText(), id.Symbol.Line, id.Symbol.Column);
            return base.VisitProcDeclare(context);
        }

        public override object VisitModuleEntry(SlangParser.ModuleEntryContext context)
        {
            _currentRoutineRow = _currentModuleRow.EntryPoint;
            return base.VisitModuleEntry(context);
        }

        public override object VisitStatementSequence(SlangParser.StatementSequenceContext context)
        {
            bool returnsValue = false;

            foreach (SlangParser.StatementContext statement in context.statement())
            {
                object result = Visit(statement);

                if (!returnsValue && result != null && result is StatementResult statementResult &&
                    statementResult.ReturnsValue)
                {
                    returnsValue = true;
                }
            }

            return new StatementResult(returnsValue);
        }

        public override object VisitConstDeclare(SlangParser.ConstDeclareContext context)
        {
            ITerminalNode id = context.Id();
            ThrowIfCorrespondingToKeyword(id);

            if (_currentRoutineRow.ContainsVariable(id.GetText()))
            {
                ThrowCompillerException(
                    _localizer[
                        "Constant '{0}' cannot be declared because another variable or constant with same name already exists.",
                        id.GetText()], id.Symbol);
            }

            SlangType constantType = (SlangType) Visit(context.simpleType());

            ParserRuleContext expressionContext = context.mathExp() ?? (ParserRuleContext) context.boolOr();
            ExpressionResult expressionResult = (ExpressionResult) Visit(expressionContext);

            if (!expressionResult.IsAssignableToType(constantType))
            {
                string expressionTypeText = GetExpressionResultTypeText(expressionResult);

                ThrowCompillerException(
                    _localizer["Cannot convert type '{0}' to constant type '{1}'.", expressionTypeText,
                        constantType], context.Start);
            }

            StatementVariableNameTableRow constantRow = new StatementVariableNameTableRow(id.Symbol.Line,
                id.Symbol.Column, constantType, id.GetText(), true, _currentRoutineRow);
            _currentRoutineRow.StatementVariables.Add(constantRow);
            _nameTableContainer.StatementVariableNameTable.Add(constantRow);

            return null;
        }

        public override object VisitScalarDeclare(SlangParser.ScalarDeclareContext context)
        {
            ITerminalNode id = context.Id();
            ThrowIfCorrespondingToKeyword(id);

            if (_currentRoutineRow.ContainsVariable(id.GetText()))
            {
                ThrowCompillerException(
                    _localizer[
                        "Variable '{0}' cannot be declared because another variable or constant with same name already exists.",
                        id.GetText()], id.Symbol);
            }

            SlangType variableType = (SlangType) Visit(context.scalarType());

            ParserRuleContext expressionContext = context.mathExp() ?? (ParserRuleContext) context.boolOr();

            if (expressionContext != null)
            {
                ExpressionResult expressionResult = (ExpressionResult) Visit(expressionContext);

                if (!expressionResult.IsAssignableToType(variableType))
                {
                    string expressionTypeText = GetExpressionResultTypeText(expressionResult);

                    ThrowCompillerException(
                        _localizer["Cannot convert type '{0}' to variable type '{1}'.", expressionTypeText,
                            variableType], context.Start);
                }
            }

            StatementVariableNameTableRow variableRow = new StatementVariableNameTableRow(id.Symbol.Line,
                id.Symbol.Column, variableType, id.GetText(), false, _currentRoutineRow);
            _currentRoutineRow.StatementVariables.Add(variableRow);
            _nameTableContainer.StatementVariableNameTable.Add(variableRow);

            return null;
        }

        public override object VisitArrayDeclare(SlangParser.ArrayDeclareContext context)
        {
            ITerminalNode id = context.Id();
            ThrowIfCorrespondingToKeyword(id);

            if (_currentRoutineRow.ContainsVariable(id.GetText()))
            {
                ThrowCompillerException(
                    _localizer[
                        "Array '{0}' cannot be declared because another variable or constant with same name already exists.",
                        id.GetText()], id.Symbol);
            }

            ArrayType arrayType = (ArrayType) Visit(context.arrayDeclareType());
            StatementVariableNameTableRow variableRow = new StatementVariableNameTableRow(id.Symbol.Line,
                id.Symbol.Column, arrayType, id.GetText(), false, _currentRoutineRow);
            _currentRoutineRow.StatementVariables.Add(variableRow);
            _nameTableContainer.StatementVariableNameTable.Add(variableRow);

            return null;
        }

        public override object VisitArrayDeclareType(SlangParser.ArrayDeclareTypeContext context)
        {
            foreach (SlangParser.ArrayDeclareDimentionContext dimention in context.arrayDeclareDimention())
            {
                Visit(dimention);
            }

            SlangType elementType = (SlangType) Visit(context.scalarType());
            return new ArrayType(elementType, context.arrayDeclareDimention().Length);
        }

        public override object VisitArrayDeclareDimention(SlangParser.ArrayDeclareDimentionContext context)
        {
            ExpressionResult expressionResult = (ExpressionResult) Visit(context.mathExp());

            if (!expressionResult.IsAssignableToType(SimpleType.Int))
            {
                string resultText = GetExpressionResultTypeText(expressionResult);
                ThrowCompillerException(
                    _localizer["Cannot convert type '{0}' to type '{1}'.", resultText, SimpleType.Int], context.Start);
            }

            return null;
        }

        public override object VisitArrayElement(SlangParser.ArrayElementContext context)
        {
            ITerminalNode id = context.Id();
            ThrowIfVariableNotDeclared(id);

            VariableNameTableRow arrayVariableRow = _currentRoutineRow.FindVariable(id.GetText());

            if (!(arrayVariableRow.Type is ArrayType))
            {
                ThrowCompillerException(_localizer["'{0}' has not an array type", id.GetText()], id.Symbol);
            }

            ArrayType arrayType = (ArrayType) arrayVariableRow.Type;

            if (arrayType.Dimentions != context.arrayDeclareDimention().Length)
            {
                ThrowCompillerException(
                    _localizer["Array '{0}' has {1} dimentions, but specified {2}.", id.GetText(), arrayType.Dimentions,
                        context.arrayDeclareDimention().Length], context.Start);
            }

            foreach (SlangParser.ArrayDeclareDimentionContext dimention in context.arrayDeclareDimention())
            {
                Visit(dimention);
            }

            return arrayType.ElementType;
        }

        public override object VisitArrayLength(SlangParser.ArrayLengthContext context)
        {
            ITerminalNode id = context.Id();
            ThrowIfVariableNotDeclared(id);

            VariableNameTableRow arrayVariableRow = _currentRoutineRow.FindVariable(id.GetText());

            if (!(arrayVariableRow.Type is ArrayType))
            {
                ThrowCompillerException(_localizer["'{0}' has not an array type.", id.GetText()], id.Symbol);
            }

            ArrayType arrayType = (ArrayType) arrayVariableRow.Type;
            int specifiedDimention = int.Parse(context.IntValue().GetText());

            if (arrayType.Dimentions <= specifiedDimention)
            {
                ThrowCompillerException(
                    _localizer["Maximum value of length index for '{0}' is {1}.", id.GetText(),
                        arrayType.Dimentions - 1], id.Symbol);
            }

            return SimpleType.Int;
        }

        public override object VisitSingleAssign(SlangParser.SingleAssignContext context)
        {
            ITerminalNode id = context.Id();
            ThrowIfVariableNotDeclared(id);

            VariableNameTableRow variableRow = _currentRoutineRow.FindVariable(id.GetText());

            if (variableRow is StatementVariableNameTableRow statementVariable && statementVariable.IsConstant)
            {
                ThrowCompillerException(_localizer["Impossible to assign a value to constant '{0}'.", variableRow.Name],
                    id.Symbol);
            }

            ExpressionResult expressionResult = (ExpressionResult) base.VisitSingleAssign(context);

            if (!expressionResult.IsAssignableToType(variableRow.Type))
            {
                string expressionTypeText = GetExpressionResultTypeText(expressionResult);

                ThrowCompillerException(
                    _localizer["Cannot convert type '{0}' to variable type '{1}'.", expressionTypeText,
                        variableRow.Type], context.Start);
            }

            return expressionResult;
        }

        public override object VisitArrayAssign(SlangParser.ArrayAssignContext context)
        {
            SlangType elementType = (SlangType) Visit(context.arrayElement());
            ExpressionResult expressionResult = (ExpressionResult) base.VisitArrayAssign(context);

            if (!expressionResult.IsAssignableToType(elementType))
            {
                string expressionTypeText = GetExpressionResultTypeText(expressionResult);

                ThrowCompillerException(
                    _localizer["Cannot convert type '{0}' to array element type '{1}'.", expressionTypeText,
                        elementType], context.Start);
            }

            return expressionResult;
        }

        public override object VisitReturn(SlangParser.ReturnContext context)
        {
            // Для функций

            if (_currentRoutineRow is FunctionNameTableRow functionRow)
            {
                if (context.exp() == null)
                {
                    ThrowCompillerException(_localizer["Missing expression in return operator."], context.Start);
                }

                ExpressionResult expressionResult = (ExpressionResult) Visit(context.exp());

                if (!expressionResult.IsAssignableToType(functionRow.ReturningType))
                {
                    string expressionTypeText = GetExpressionResultTypeText(expressionResult);

                    ThrowCompillerException(
                        _localizer["Cannot convert type '{0}' to variable type '{1}'.", expressionTypeText,
                            functionRow.ReturningType], context.Start);
                }

                return new StatementResult(true);
            }

            // Для процедур
            if (context.exp() != null)
            {
                ThrowCompillerException(_localizer["Only functions can return a value."], context.Start);
            }

            return new StatementResult(true);
        }

        public override object VisitInput(SlangParser.InputContext context)
        {
            ITerminalNode id = context.Id();
            ThrowIfVariableNotDeclared(id);

            VariableNameTableRow variableRow = _currentRoutineRow.FindVariable(id.GetText());

            if (!SimpleType.Real.IsAssignable(variableRow.Type) && !SimpleType.Bool.IsAssignable(variableRow.Type))
            {
                ThrowCompillerException(_localizer["Cannot input variable with type '{0}'.", variableRow.Type],
                    id.Symbol);
            }

            return null;
        }

        //public override object VisitCall(SlangParser.CallContext context)
        //{
        //    // Если есть модуль, то это другой модуль, иначе функция из текущего или переменная
        //    if (context.Id().Length > 1)
        //    {
        //        ITerminalNode moduleId = context.Id(0);

        //        ModuleNameTableRow moduleRow = _nameTableContainer.FindModule(moduleId.GetText());

        //        if (moduleRow == null)
        //        {
        //            ThrowCompillerException(string.Format(Resources.Resources.ModuleIsNotDeclared, moduleId.GetText()),
        //                moduleId.Symbol);
        //            return null;
        //        }

        //        if (!_currentModuleRow.IsImported(moduleId.GetText()))
        //        {
        //            ThrowCompillerException(string.Format(Resources.Resources.ModuleIsNotImported, moduleId.GetText()),
        //                moduleId.Symbol);
        //        }

        //        ITerminalNode functionId = context.Id(1);
        //        IList<ExpressionResult> callArgResults = (IList<ExpressionResult>) Visit(context.callArgList());
        //        RoutineNameTableRow routineRow = moduleRow.FindSuitableRoutine(functionId.GetText(),
        //            callArgResults.Select(x => x.SlangType).ToList());

        //        switch (routineRow)
        //        {
        //            case null:
        //                ThrowCompillerException(
        //                    string.Format(Resources.Resources.ModuleDoesNotContainsRoutine, moduleId.GetText(),
        //                        functionId.GetText()), functionId.Symbol);
        //                break;

        //            case FunctionNameTableRow functionRow:
        //                return functionRow.ReturningType;

        //            default:
        //                return null;
        //        }
        //    }
        //    else
        //    {
        //        ITerminalNode functorId = context.Id(0);
        //        IList<SlangType> callArgTypes = (IList<SlangType>) Visit(context.callArgList());
        //        RoutineNameTableRow routineRow =
        //            _currentModuleRow.FindSuitableRoutine(functorId.GetText(), callArgTypes);

        //        if (routineRow != null)
        //        {
        //            if (routineRow is FunctionNameTableRow functionRow)
        //            {
        //                return functionRow.ReturningType;
        //            }

        //            return null;
        //        }

        //        if (_currentModuleRow.ContainsRoutine(functorId.GetText()))
        //        {
        //            ThrowCompillerException(
        //                string.Format(Resources.Resources.ModuleDoesNotContainsRoutine, _slangModule.ModuleName,
        //                    functorId.GetText()), functorId.Symbol);
        //        }

        //        ThrowIfNotDeclared(functorId);
        //        VariableNameTableRow functorRow = _currentRoutineRow.FindVariable(functorId.GetText());

        //        if (!(functorRow.Type is RoutineType routineType))
        //        {
        //            ThrowCompillerException(
        //                string.Format(Resources.Resources.VariableIsNotFunctor, functorId.GetText()), functorId.Symbol);
        //            return null;
        //        }

        //        if (!routineType.IsSuitable(callArgTypes))
        //        {
        //            ThrowCompillerException(
        //                string.Format(Resources.Resources.RoutineDoesNotTakesArgs, functorId.GetText()),
        //                functorId.Symbol);
        //        }

        //        if (routineType is FunctionType functionType)
        //        {
        //            return functionType.ReturningType;
        //        }
        //    }

        //    return null;
        //}

        //public override object VisitCallArgList(SlangParser.CallArgListContext context)
        //{
        //    IList<ExpressionResult> argTypes =
        //        context.callArg().Select(arg => (ExpressionResult) Visit(arg)).ToList();
        //    return argTypes;
        //}

        public override object VisitIfElse(SlangParser.IfElseContext context)
        {
            Visit(context.boolOr());

            StatementResult ifResult = (StatementResult) Visit(context.statementSequence(0));
            StatementResult elseResult = (StatementResult) Visit(context.statementSequence(1));

            return new StatementResult(ifResult.ReturnsValue && elseResult.ReturnsValue);
        }

        public override object VisitMathExpSum(SlangParser.MathExpSumContext context)
        {
            return VisitBinaryExpression(context, CanMathBinaryOperatorBeApplied, CalculateBinaryMathType);
        }

        public override object VisitMathExpSub(SlangParser.MathExpSubContext context)
        {
            return VisitBinaryExpression(context, CanMathBinaryOperatorBeApplied, CalculateBinaryMathType);
        }

        public override object VisitMathTermMul(SlangParser.MathTermMulContext context)
        {
            return VisitBinaryExpression(context, CanMathBinaryOperatorBeApplied, CalculateBinaryMathType);
        }

        public override object VisitMathTermDiv(SlangParser.MathTermDivContext context)
        {
            return VisitBinaryExpression(context, CanMathBinaryOperatorBeApplied, CalculateBinaryMathType);
        }

        public override object VisitMathTermMod(SlangParser.MathTermModContext context)
        {
            return VisitBinaryExpression(context,
                (left, right) => left.IsAssignableToType(SimpleType.Int) && right.IsAssignableToType(SimpleType.Int),
                CalculateBinaryMathType);
        }

        public override object VisitMathFactorBrackets(SlangParser.MathFactorBracketsContext context)
        {
            return Visit(context.mathExp());
        }

        public override object VisitMathFactorUnaryPlus(SlangParser.MathFactorUnaryPlusContext context)
        {
            ExpressionResult result = (ExpressionResult) Visit(context.mathFactor());

            if (result.IsAssignableToType(SimpleType.Real))
            {
                return result;
            }

            string op = context.GetChild(0).GetText();
            ThrowCompillerException(
                _localizer["Operator '{0}' can't be applied to operand of type '{1}'", op,
                    result.PossibleTypes.First()], context.Start);

            return null;
        }

        public override object VisitMathFactorUnaryMinus(SlangParser.MathFactorUnaryMinusContext context)
        {
            ExpressionResult result = (ExpressionResult) Visit(context.mathFactor());

            if (result.IsAssignableToType(SimpleType.Real))
            {
                return result;
            }

            string op = context.GetChild(0).GetText();

            ThrowCompillerException(
                _localizer["Operator '{0}' can't be applied to operand of type '{1}'.", op,
                    result.PossibleTypes.First()], context.Start);

            return null;
        }

        public override object VisitLogicOr(SlangParser.LogicOrContext context)
        {
            return VisitBinaryExpression(context, CanBoolBinaryOperatorBeApplied, CalculateBinaryBoolType);
        }

        public override object VisitLogicAnd(SlangParser.LogicAndContext context)
        {
            return VisitBinaryExpression(context, CanBoolBinaryOperatorBeApplied, CalculateBinaryBoolType);
        }

        public override object VisitBoolEqual(SlangParser.BoolEqualContext context)
        {
            return VisitBinaryExpression(context, CanBoolBinaryOperatorBeApplied, CalculateBinaryBoolType);
        }

        public override object VisitMathEqual(SlangParser.MathEqualContext context)
        {
            return VisitBinaryExpression(context, CanMathBinaryOperatorBeApplied, CalculateBinaryBoolType);
        }

        public override object VisitBoolNotEqual(SlangParser.BoolNotEqualContext context)
        {
            return VisitBinaryExpression(context, CanBoolBinaryOperatorBeApplied, CalculateBinaryBoolType);
        }

        public override object VisitMathNotEqual(SlangParser.MathNotEqualContext context)
        {
            return VisitBinaryExpression(context, CanMathBinaryOperatorBeApplied, CalculateBinaryBoolType);
        }

        public override object VisitBigger(SlangParser.BiggerContext context)
        {
            return VisitBinaryExpression(context, CanMathBinaryOperatorBeApplied, CalculateBinaryBoolType);
        }

        public override object VisitLesser(SlangParser.LesserContext context)
        {
            return VisitBinaryExpression(context, CanMathBinaryOperatorBeApplied, CalculateBinaryBoolType);
        }

        public override object VisitBiggerOrEqual(SlangParser.BiggerOrEqualContext context)
        {
            return VisitBinaryExpression(context, CanMathBinaryOperatorBeApplied, CalculateBinaryBoolType);
        }

        public override object VisitLesserOrEqual(SlangParser.LesserOrEqualContext context)
        {
            return VisitBinaryExpression(context, CanMathBinaryOperatorBeApplied, CalculateBinaryBoolType);
        }

        public override object VisitNot(SlangParser.NotContext context)
        {
            ExpressionResult result = (ExpressionResult) Visit(context.expAtom());

            if (result.IsAssignableToType(SimpleType.Bool))
            {
                return result;
            }

            string op = context.GetChild(0).GetText();

            ThrowCompillerException(
                _localizer["Operator '{0}' can't be applied to operand of type '{1}'", op,
                    result.PossibleTypes.First()],
                context.Start);
            return null;
        }

        public override object VisitBoolAtomBrackets(SlangParser.BoolAtomBracketsContext context)
        {
            return Visit(context.boolOr());
        }

        public override object VisitBoolAtomBracketsNot(SlangParser.BoolAtomBracketsNotContext context)
        {
            ExpressionResult result = (ExpressionResult) Visit(context.boolOr());

            if (result.IsAssignableToType(SimpleType.Bool))
            {
                return result;
            }

            string op = context.GetChild(0).GetText();
            ThrowCompillerException(
                _localizer["Operator '{0}' can't be applied to operand of type '{1}'", op,
                    result.PossibleTypes.First()], context.Start);

            return null;
        }

        public override object VisitExpAtom(SlangParser.ExpAtomContext context)
        {
            ExpressionResult result;

            if (context.call() != null)
            {
                SlangType slangType = (SlangType) Visit(context.call());

                if (slangType == null)
                {
                    SlangParser.IdContext routineId = context.call().id();
                    ThrowCompillerException(
                        _localizer["'{0}' is not a function and can't be used in expression.", routineId.GetText()],
                        routineId.Start);
                }

                result = new ExpressionResult(ExpressionType.Call, slangType);
            }
            else if (context.arrayLength() != null)
            {
                SlangType lengthType = (SlangType) Visit(context.arrayLength());
                result = new ExpressionResult(ExpressionType.ArrayLength, lengthType);
            }
            else if (context.arrayElement() != null)
            {
                SlangType elementType = (SlangType) Visit(context.arrayElement());
                result = new ExpressionResult(ExpressionType.ArrayElement, elementType);
            }
            else if (context.IntValue() != null)
            {
                result = new ExpressionResult(ExpressionType.Value, SimpleType.Int);
            }
            else if (context.RealValue() != null)
            {
                result = new ExpressionResult(ExpressionType.Value, SimpleType.Real);
            }
            else if (context.BoolValue() != null)
            {
                result = new ExpressionResult(ExpressionType.Value, SimpleType.Bool);
            }
            else if (context.id() != null)
            {
                result = (ExpressionResult) Visit(context.id());
            }
            else
            {
                ThrowCompillerException(_localizer["Unknown type in expression."], context.Start);
                result = null;
            }

            return result;
        }

        public override object VisitId(SlangParser.IdContext context)
        {
            ITerminalNode[] ids = context.Id();

            if (ids.Length > 1)
            {
                ITerminalNode moduleId = ids[0];

                if (!_currentModuleRow.IsImported(moduleId.GetText()))
                {
                    ThrowCompillerException(_localizer["Module '{0}' is not imported.", moduleId.GetText()],
                        moduleId.Symbol);
                }

                ModuleNameTableRow moduleRow = _nameTableContainer.ModuleNameTable.FindModule(moduleId.GetText());

                if (moduleRow == null)
                {
                    ThrowCompillerException(_localizer["Module '{0}' is not defined.", moduleId.GetText()],
                        moduleId.Symbol);

                    return null;
                }

                ITerminalNode id = ids[1];
                IEnumerable<RoutineNameTableRow> routineRows = moduleRow.FindRoutinesByName(id.GetText());
                SlangType[] routineTypes = routineRows.Select(x => x.ToSlangType()).ToArray();

                if (routineTypes == null || routineTypes.Length == 0)
                {
                    ThrowCompillerException(
                        _localizer["Function or procedure '{0}' is not declared in module '{1}'.", id.GetText(),
                            moduleId.GetText()], id.Symbol);
                }

                return new ExpressionResult(ExpressionType.Routine, routineTypes);
            }
            else
            {
                ITerminalNode id = ids[0];
                VariableNameTableRow variableRow = _currentRoutineRow.FindVariable(id.GetText());

                if (variableRow != null)
                {
                    return new ExpressionResult(ExpressionType.Variable, variableRow.Type);
                }

                IEnumerable<RoutineNameTableRow> routineRows = _currentModuleRow.FindRoutinesByName(id.GetText());
                SlangType[] routineTypes = routineRows.Select(x => x.ToSlangType()).ToArray();

                if (routineTypes != null && routineTypes.Length > 0)
                {
                    return new ExpressionResult(ExpressionType.Routine, routineTypes);
                }

                ThrowCompillerException(
                    _localizer["Name '{0}' is not declared in current context.", id.GetText()], id.Symbol);
                return null;
            }
        }

        #region Private

        private static bool IsCorrespondingToKeyword(IParseTree id)
        {
            return Keywords.Contains(id.GetText());
        }

        private void ThrowIfCorrespondingToKeyword(ITerminalNode id)
        {
            if (!IsCorrespondingToKeyword(id))
            {
                return;
            }

            IToken symbol = id.Symbol;
            ThrowCompillerException(_localizer["Name {0} corresponds to keyword.", id.GetText()], symbol);
        }

        private void ThrowIfVariableNotDeclared(ITerminalNode variableId)
        {
            string variableName = variableId.GetText();

            if (!_currentRoutineRow.ContainsVariable(variableName))
            {
                ThrowCompillerException(
                    _localizer["Variable '{0}' is not declared in this context.", variableName],
                    variableId.Symbol);
            }
        }

        private static bool CanMathBinaryOperatorBeApplied(ExpressionResult left, ExpressionResult right)
        {
            return left.IsAssignableToType(SimpleType.Real) && right.IsAssignableToType(SimpleType.Real);
        }

        private static bool CanBoolBinaryOperatorBeApplied(ExpressionResult left, ExpressionResult right)
        {
            return left.IsAssignableToType(SimpleType.Bool) && right.IsAssignableToType(SimpleType.Bool);
        }

        private static SlangType CalculateBinaryMathType(ExpressionResult left, ExpressionResult right)
        {
            if (left.PossibleTypes.First().Equals(SimpleType.Int) && right.PossibleTypes.First().Equals(SimpleType.Int))
            {
                return SimpleType.Int;
            }

            return SimpleType.Real;
        }

        private static SlangType CalculateBinaryBoolType(ExpressionResult left, ExpressionResult right)
        {
            return SimpleType.Bool;
        }

        /// <summary>
        ///     По-умолчанию - проверкая для математических выражений
        /// </summary>
        private object VisitBinaryExpression(ParserRuleContext context,
            Func<ExpressionResult, ExpressionResult, bool> applyChecker,
            Func<ExpressionResult, ExpressionResult, SlangType> resultTypeCalculator)
        {
            ExpressionResult leftResult = (ExpressionResult) Visit(context.GetChild(0));
            ExpressionResult rightResult = (ExpressionResult) Visit(context.GetChild(2));

            if (!applyChecker(leftResult, rightResult))
            {
                string @operator = context.GetChild(1).GetText();
                string leftType = GetExpressionResultTypeText(leftResult);
                string rightType = GetExpressionResultTypeText(rightResult);

                ThrowCompillerException(
                    _localizer["Operator '{0}' can't be applied to operands of type '{1}' and '{2}'.", @operator,
                        leftType, rightType], context.Start);
            }

            SlangType type = resultTypeCalculator(leftResult, rightResult);
            return new ExpressionResult(ExpressionType.Expression, type);
        }

        private string GetExpressionResultTypeText(ExpressionResult result)
        {
            switch (result.ExpressionType)
            {
                case ExpressionType.Routine:
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(result.PossibleTypes[0]);

                    for (int i = 1; i < result.PossibleTypes.Length; i++)
                    {
                        builder.Append("; ");
                        builder.Append(result.PossibleTypes[i]);
                    }

                    return builder.ToString();
                }

                default:
                    return result.PossibleTypes.FirstOrDefault().ToString();
            }
        }

        #endregion
    }
}