using Antlr4.Runtime.Tree;
using CompillerServices.Exceptions;
using CompillerServices.Frontend.NameTables;
using CompillerServices.IO;
using SlangGrammar;

namespace CompillerServices.Frontend
{
    internal class SecondStepVisitor : SlangBaseVisitor<object>
    {
        private readonly INameTableContainer _nameTableContainer;
        private readonly SlangModule _slangModule;

        public SecondStepVisitor(INameTableContainer nameTableContainer, SlangModule slangModule)
        {
            _nameTableContainer = nameTableContainer;
            _slangModule = slangModule;
        }

        public override object VisitModuleImports(SlangParser.ModuleImportsContext context)
        {
            foreach (ITerminalNode module in context.Id())
            {
                string moduleName = module.GetText();

                if (!_nameTableContainer.ModuleNameTable.Contains(moduleName))
                {
                    throw new CompillerException(string.Format(Resources.Resources.ModuleIsNotDeclared, moduleName),
                        _slangModule.ModuleName, module.Symbol.Line, module.Symbol.Column);
                }
            }

            return base.VisitModuleImports(context);
        }
    }
}