using System.Text;
using Antlr4.Runtime.Tree;
using CompillerServices.Exceptions;
using CompillerServices.Frontend.NameTables;
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

        public override object VisitModule(SlangParser.ModuleContext context)
        {
            ITerminalNode id = context.Id();
            string moduleName = id.GetText();

            if (moduleName != _slangModule.ModuleName)
            {
                throw new ModuleAndFileMatchException(_slangModule.ModuleFile, moduleName, id.Symbol.Line,
                    id.Symbol.Column);
            }

            _moduleRow = new ModuleNameTableRow(moduleName);
            _nameTableContainer.ModuleNameTable.Add(_moduleRow);

            base.VisitModule(context);
            return null;
        }

        public override object VisitFunc(SlangParser.FuncContext context)
        {
            string modifier = context.ModuleAccessModifier().GetText();
            string type = GetRuleTypeString(context.arrayOrSimpleType());
            string name = context.Id().GetText();

            FunctionNameTableRow functionRow = new FunctionNameTableRow(modifier, type, name, _moduleRow);
            _nameTableContainer.FunctionNameTable.Add(functionRow);
            _moduleRow.Functions.Add(functionRow);

            _currentSubprogram = functionRow;

            Visit(context.argList());
            Visit(context.statementBlock());

            return null;
        }

        public override object VisitProc(SlangParser.ProcContext context)
        {
            string modifier = context.ModuleAccessModifier().GetText();
            string name = context.Id().GetText();

            ProcedureNameTableRow procedureRow = new ProcedureNameTableRow(modifier, name, _moduleRow);
            _nameTableContainer.ProcedureNameTable.Add(procedureRow);
            _moduleRow.Procedures.Add(procedureRow);

            _currentSubprogram = procedureRow;

            Visit(context.argList());
            Visit(context.statementBlock());

            return null;
        }

        public override object VisitArgList(SlangParser.ArgListContext context)
        {
            SlangParser.ArgPassModifierContext[] modifiers = context.argPassModifier();
            SlangParser.ArrayOrSimpleTypeContext[] types = context.arrayOrSimpleType();
            ITerminalNode[] names = context.Id();

            for (int i = 0; i < names.Length; i++)
            {
                string modifier = modifiers[i].ArgPassModifier().GetText();
                string type = GetRuleTypeString(types[i]);
                string name = names[i].GetText();

                ArgumentNameTableRow argument = new ArgumentNameTableRow(modifier, type, name, _currentSubprogram);
                _nameTableContainer.ArgumentNameTable.TryAdd(argument);
                _currentSubprogram.Arguments.Add(argument);
            }

            return null;
        }

        public override object VisitDeclare(SlangParser.DeclareContext context)
        {
            string type = GetRuleTypeString(context.arrayOrSimpleType());
            string name = context.Id().GetText();

            VariableNameTableRow variable = new VariableNameTableRow(type, name, _currentSubprogram);
            _nameTableContainer.VariableNameTable.TryAdd(variable);
            _currentSubprogram.Variables.Add(variable);

            return null;
        }

        public override object VisitArrayType(SlangParser.ArrayTypeContext context)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(context.Type().GetText());

            for (int i = 0; i < context.ArrayTypeBrackets().Length; i++)
            {
                builder.Append("[]");
            }

            return builder.ToString();
        }

        private string GetRuleTypeString(SlangParser.ArrayOrSimpleTypeContext context)
        {
            return context.arrayType() != null ? (string) Visit(context.arrayType()) : context.Type().ToString();
        }
    }
}