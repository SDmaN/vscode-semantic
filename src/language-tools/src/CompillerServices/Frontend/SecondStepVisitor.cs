using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CompillerServices.Exceptions;
using CompillerServices.Frontend.NameTables;
using CompillerServices.Frontend.NameTables.Types;
using CompillerServices.IO;
using SlangGrammar;

namespace CompillerServices.Frontend
{
    internal class SecondStepVisitor : BaseStepVisitor
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
        private readonly INameTableContainer _nameTableContainer;
        private readonly SlangModule _slangModule;
        private RoutineNameTableRow _currentRoutineRow;

        public SecondStepVisitor(INameTableContainer nameTableContainer, SlangModule slangModule)
            : base(slangModule)
        {
            _nameTableContainer = nameTableContainer;
            _slangModule = slangModule;
            _currentModuleRow = _nameTableContainer.ModuleNameTable.GetModuleRow(_slangModule.ModuleName);
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
                    ThrowCompillerException(string.Format(Resources.Resources.ModuleIsNotDeclared, moduleName),
                        importingModule.Symbol);
                }

                if (alreadyImported.Contains(moduleName))
                {
                    ThrowCompillerException(string.Format(Resources.Resources.ModuleAlreadyImported, moduleName),
                        importingModule.Symbol);
                }

                if (moduleName == _currentModuleRow.ModuleName)
                {
                    throw new CompillerException(Resources.Resources.ImportingCurrentModuleError,
                        _slangModule.ModuleName, importingModule.Symbol.Line, importingModule.Symbol.Column);
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
            return base.VisitFuncDeclare(context);
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

        public override object VisitMathAtom(SlangParser.MathAtomContext context)
        {
            ExpressionResult result;

            if (context.call() != null)
            {
                SlangType slangType = (SlangType) Visit(context.call());

                if (slangType == null)
                {
                    ITerminalNode routineId = context.call().Id(1);
                    ThrowCompillerException(
                        string.Format(Resources.Resources.ProcedureCantBeUsedInExpression, routineId.GetText()),
                        routineId.Symbol);
                }

                result = new ExpressionResult((SlangType) Visit(context.call()), ExpressionType.Call);
            }
            else if (context.arrayLength() != null)
            {
                result = new ExpressionResult(SimpleType.Int, ExpressionType.ArrayLength);
            }
            else if (context.arrayElement() != null)
            {
                SlangType slangType = (SlangType) Visit(context.arrayElement());
            }
            else if (context.IntValue() != null)
            {
                result = new ExpressionResult(SimpleType.Int, ExpressionType.Value);
            }
            else if (context.RealValue() != null)
            {
                result = new ExpressionResult(SimpleType.Real, ExpressionType.Value);
            }
            else if (context.Id() != null)
            {
                ITerminalNode id = context.Id();
                ThrowIfNotDeclared(id);

                VariableNameTableRow variableRow = _currentRoutineRow.FindVariable(id.GetText());

                if (!SimpleType.Real.IsAssignable(variableRow.Type))
                {
                    ThrowCompillerException(string.Format(Resources.Resources.VariableIsNotMath, id.GetText()),
                        id.Symbol);
                }

                result = new ExpressionResult(variableRow.Type, ExpressionType.Variable);
            }
            else
            {
                throw new CompillerException("Unknown type in math exp!", _slangModule.ModuleName, -1, -1);
            }

            return result;
        }

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

            ThrowCompillerException(string.Format(Resources.Resources.CorrespondingToKeywordError, id.GetText()),
                symbol);
        }

        private void ThrowIfNotDeclared(ITerminalNode variableId)
        {
            if (!_currentRoutineRow.ContainsVariable(variableId.GetText()))
            {
                ThrowCompillerException(string.Format(Resources.Resources.VariableIsNotDeclared, variableId.GetText()),
                    variableId.Symbol);
            }
        }
    }
}