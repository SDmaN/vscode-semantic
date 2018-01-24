using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CompillerServices.Exceptions;
using CompillerServices.Frontend.NameTables;
using CompillerServices.Frontend.NameTables.Types;
using CompillerServices.IO;
using SlangGrammar;

namespace CompillerServices.Frontend
{
    internal class FirstStepVisitor : SlangBaseVisitor<object>
    {
        private readonly INameTableContainer _nameTableContainer;
        private readonly SlangModule _slangModule;
        private SubprogramNameTableRow _currentSubprogram;
        private ModuleNameTableRow _moduleRow;

        public FirstStepVisitor(INameTableContainer nameTableContainer, SlangModule slangModule)
        {
            _nameTableContainer = nameTableContainer;
            _slangModule = slangModule;
        }

        public override object VisitStart(SlangParser.StartContext context)
        {
            Visit(context.module());
            Visit(context.moduleImports());

            return null;
        }

        public override object VisitSimpleType(SlangParser.SimpleTypeContext context)
        {
            return new SimpleType(context.SimpleType().GetText());
        }

        public override object VisitFuncType(SlangParser.FuncTypeContext context)
        {
            IEnumerable<RoutineTypeArg> routineTypeArgs = (IEnumerable<RoutineTypeArg>) Visit(context.routineArgList());
            SlangType returningType = (SlangType) Visit(context.type());

            return new FunctionType(returningType, routineTypeArgs);
        }

        public override object VisitProcType(SlangParser.ProcTypeContext context)
        {
            IEnumerable<RoutineTypeArg> routineTypeArgs = (IEnumerable<RoutineTypeArg>) Visit(context.routineArgList());
            return new ProcedureType(routineTypeArgs);
        }

        public override object VisitRoutineArgList(SlangParser.RoutineArgListContext context)
        {
            IList<RoutineTypeArg> routineTypeArgs = new List<RoutineTypeArg>(context.routineArg().Length);

            foreach (SlangParser.RoutineArgContext arg in context.routineArg())
            {
                routineTypeArgs.Add((RoutineTypeArg) Visit(arg));
            }

            return routineTypeArgs;
        }

        public override object VisitRoutineArg(SlangParser.RoutineArgContext context)
        {
            string modifier = context.ArgPassModifier().GetText();
            SlangType type = (SlangType) Visit(context.type());

            return new RoutineTypeArg(modifier, type);
        }

        public override object VisitArrayType(SlangParser.ArrayTypeContext context)
        {
            SlangType elementType = (SlangType) Visit(context.scalarType());
            int dimentions = context.arrayDimention().Length;

            return new ArrayType(elementType, dimentions);
        }

        public override object VisitModule(SlangParser.ModuleContext context)
        {
            ITerminalNode id = context.Id();

            string moduleName = id.GetText();

            if (moduleName != _slangModule.ModuleName)
            {
                throw new ModuleAndFileMismatchException(_slangModule.ModuleFile, moduleName, id.Symbol.Line,
                    id.Symbol.Column);
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
            foreach (ITerminalNode module in context.Id())
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
            _nameTableContainer.FunctionNameTable.Add(functionRow);
            _moduleRow.Functions.Add(functionRow);

            _currentSubprogram = functionRow;

            Visit(context.routineDeclareArgList());
            Visit(context.statementSequence());

            return null;
        }

        public override object VisitProcDeclare(SlangParser.ProcDeclareContext context)
        {
            string modifier = context.AccessModifier().GetText();
            
            string name = context.Id().GetText();
            IToken nameSymbol = context.Id().Symbol;

            ProcedureNameTableRow row =
                new ProcedureNameTableRow(nameSymbol.Line, nameSymbol.Column, modifier, name, _moduleRow);
            _nameTableContainer.ProcedureNameTable.Add(row);
            _moduleRow.Procedures.Add(row);

            _currentSubprogram = row;

            Visit(context.routineDeclareArgList());
            Visit(context.statementSequence());

            return null;
        }

        public override object VisitRoutineDeclareArg(SlangParser.RoutineDeclareArgContext context)
        {
            ITerminalNode modifier = context.ArgPassModifier();
            SlangType type = (SlangType) Visit(context.type());
            ITerminalNode id = context.Id();
            IToken idSymbol = id.Symbol;

            ArgumentNameTableRow row = new ArgumentNameTableRow(idSymbol.Line, idSymbol.Column,
                modifier.GetText(), type, id.GetText(), _currentSubprogram);

            _nameTableContainer.ArgumentNameTable.Add(row);
            _currentSubprogram.Arguments.Add(row);

            return null;
        }
    }
}