using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CompillerServices.Exceptions;
using CompillerServices.Frontend.NameTables;
using CompillerServices.Frontend.NameTables.Types;
using CompillerServices.IO;
using Microsoft.Extensions.Localization;
using SlangGrammar;

namespace CompillerServices.Frontend
{
    internal class FirstStepVisitor : BaseStepVisitor
    {
        private readonly IStringLocalizer<FirstStepVisitor> _localizer;
        private readonly INameTableContainer _nameTableContainer;
        private readonly SlangModule _slangModule;
        private RoutineNameTableRow _currentRoutine;
        private ModuleNameTableRow _moduleRow;

        public FirstStepVisitor(IStringLocalizer<FirstStepVisitor> localizer, INameTableContainer nameTableContainer,
            SlangModule slangModule)
            : base(slangModule)
        {
            _localizer = localizer;
            _nameTableContainer = nameTableContainer;
            _slangModule = slangModule;
        }

        public override object VisitStart(SlangParser.StartContext context)
        {
            Visit(context.module());
            Visit(context.moduleImports());

            return null;
        }

        public override object VisitModule(SlangParser.ModuleContext context)
        {
            ITerminalNode id = context.Id();

            string moduleName = id.GetText();

            if (moduleName != _slangModule.ModuleName)
            {
                throw new ModuleAndFileMismatchException(
                    _localizer["Module name '{0}' does not match file {1}.", moduleName, _slangModule.ModuleFile.Name],
                    _slangModule.ModuleFile, _slangModule.ModuleName, id.Symbol.Line, id.Symbol.Column);
            }

            // Поскольку название файлов всегда разное, а название модуля должно совпадать с назаванием файла
            // то и проверять наличие в таблице символов не нужно

            IToken symbol = id.Symbol;
            _moduleRow = new ModuleNameTableRow(symbol.Line, symbol.Column, moduleName);
            _nameTableContainer.ModuleNameTable.Add(_moduleRow);

            base.VisitModule(context);
            return null;
        }
        

        public override object VisitModuleImports(SlangParser.ModuleImportsContext context)
        {
            foreach (ITerminalNode module in context.moduleImport().Select(x => x.Id()))
            {
                _moduleRow.ImportingModules.Add(module.GetText());
            }

            return null;
        }

        public override object VisitFuncDeclare(SlangParser.FuncDeclareContext context)
        {
            string modifier = context.AccessModifier().GetText();
            SlangType type = (SlangType) Visit(context.type());

            string name = context.Id().GetText();
            IToken nameSymbol = context.Id().Symbol;

            FunctionNameTableRow functionRow =
                new FunctionNameTableRow(nameSymbol.Line, nameSymbol.Column, modifier, type, name, _moduleRow);
            _currentRoutine = functionRow;

            ICollection<ArgumentNameTableRow> argRows =
                (ICollection<ArgumentNameTableRow>) Visit(context.routineDeclareArgList());

            if (_moduleRow.ContainsExactRoutine(name, argRows.Select(x => x.Type).ToList()))
            {
                ThrowCompillerException(
                    _localizer["Function or procedure '{0}' with same signature already defined.", name], nameSymbol);
            }

            foreach (ArgumentNameTableRow argRow in argRows)
            {
                _nameTableContainer.ArgumentNameTable.Add(argRow);
                functionRow.Arguments.Add(argRow);
            }

            _nameTableContainer.FunctionNameTable.Add(functionRow);
            _moduleRow.Functions.Add(functionRow);

            Visit(context.statementSequence());

            return null;
        }

        public override object VisitProcDeclare(SlangParser.ProcDeclareContext context)
        {
            string modifier = context.AccessModifier().GetText();

            string name = context.Id().GetText();
            IToken nameSymbol = context.Id().Symbol;

            ProcedureNameTableRow procedureRow =
                new ProcedureNameTableRow(nameSymbol.Line, nameSymbol.Column, modifier, name, _moduleRow);
            _currentRoutine = procedureRow;

            ICollection<ArgumentNameTableRow> argRows =
                (ICollection<ArgumentNameTableRow>) Visit(context.routineDeclareArgList());

            if (_moduleRow.ContainsExactRoutine(name, argRows.Select(x => x.Type).ToList()))
            {
                ThrowCompillerException(
                    _localizer["Function or procedure '{0}' with same signature already defined.", name], nameSymbol);
            }

            foreach (ArgumentNameTableRow argRow in argRows)
            {
                _nameTableContainer.ArgumentNameTable.Add(argRow);
                procedureRow.Arguments.Add(argRow);
            }

            _nameTableContainer.ProcedureNameTable.Add(procedureRow);
            _moduleRow.Procedures.Add(procedureRow);

            Visit(context.routineDeclareArgList());
            Visit(context.statementSequence());

            return null;
        }

        public override object VisitRoutineDeclareArgList(SlangParser.RoutineDeclareArgListContext context)
        {
            ICollection<ArgumentNameTableRow> argRows = new List<ArgumentNameTableRow>();

            foreach (SlangParser.RoutineDeclareArgContext arg in context.routineDeclareArg())
            {
                ArgumentNameTableRow argRow = (ArgumentNameTableRow) Visit(arg);

                if (argRows.Any(x => x.Name == argRow.Name))
                {
                    ThrowCompillerException(_localizer["Argument '{0}' already defined.", argRow.Name],
                        arg.Id().Symbol);
                }

                argRows.Add(argRow);
            }

            return argRows;
        }

        public override object VisitRoutineDeclareArg(SlangParser.RoutineDeclareArgContext context)
        {
            ITerminalNode modifier = context.ArgPassModifier();
            SlangType type = (SlangType) Visit(context.type());
            ITerminalNode id = context.Id();
            IToken idSymbol = id.Symbol;

            ArgumentNameTableRow row = new ArgumentNameTableRow(idSymbol.Line, idSymbol.Column,
                modifier.GetText(), type, id.GetText(), _currentRoutine);

            return row;
        }

        public override object VisitModuleEntry(SlangParser.ModuleEntryContext context)
        {
            ITerminalNode start = context.Start();
            _moduleRow.EntryPoint = new EntryPointNameTableRow(start.Symbol.Line, start.Symbol.Column, _moduleRow);
            _nameTableContainer.EntryPointNameTable.Add(_moduleRow.EntryPoint);

            return base.VisitModuleEntry(context);
        }
    }
}