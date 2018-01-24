using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Tree;
using CompillerServices.Exceptions;
using CompillerServices.Frontend.NameTables;
using CompillerServices.IO;
using SlangGrammar;

namespace CompillerServices.Frontend
{
    internal class SecondStepVisitor : SlangBaseVisitor<object>
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
            "const"
        };
        
        private readonly INameTableContainer _nameTableContainer;
        private readonly SlangModule _slangModule;
        private readonly ModuleNameTableRow _moduleRow;

        public SecondStepVisitor(INameTableContainer nameTableContainer, SlangModule slangModule)
        {
            _nameTableContainer = nameTableContainer;
            _slangModule = slangModule;
            _moduleRow = _nameTableContainer.ModuleNameTable.GetModuleRow(_slangModule.ModuleName);
        }

        /*public override object VisitModuleImports(SlangParser.ModuleImportsContext context)
        {
            ICollection<string> alreadyImported = new HashSet<string>();

            foreach (ITerminalNode module in context.Id())
            {
                string moduleName = module.GetText();

                if (!_nameTableContainer.ModuleNameTable.Contains(moduleName))
                {
                    throw new CompillerException(string.Format(Resources.Resources.ModuleIsNotDeclared, moduleName),
                        _slangModule.ModuleName, module.Symbol.Line, module.Symbol.Column);
                }

                if (alreadyImported.Contains(moduleName))
                {
                    throw new CompillerException(string.Format(Resources.Resources.ModuleAlreadyImported, moduleName),
                        _slangModule.ModuleName, module.Symbol.Line, module.Symbol.Column);
                }

                if (moduleName == _moduleRow.ModuleName)
                {
                    throw new CompillerException(Resources.Resources.ImportingCurrentModuleError,
                        _slangModule.ModuleName, module.Symbol.Line, module.Symbol.Column);
                }

                alreadyImported.Add(moduleName);
            }

            return base.VisitModuleImports(context);
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

        private string GetRuleTypeString(SlangParser.ArrayOrSimpleTypeContext context)
        {
            return context.arrayType() != null ? (string) Visit(context.arrayType()) : context.Type().ToString();
        }*/
    }
}