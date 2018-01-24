using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
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
            "const",

            "class"
        };

        private readonly ModuleNameTableRow _moduleRow;

        private readonly INameTableContainer _nameTableContainer;
        private readonly SlangModule _slangModule;

        public SecondStepVisitor(INameTableContainer nameTableContainer, SlangModule slangModule)
        {
            _nameTableContainer = nameTableContainer;
            _slangModule = slangModule;
            _moduleRow = _nameTableContainer.ModuleNameTable.GetModuleRow(_slangModule.ModuleName);
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
                    throw new CompillerException(string.Format(Resources.Resources.ModuleIsNotDeclared, moduleName),
                        _slangModule.ModuleName, importingModule.Symbol.Line, importingModule.Symbol.Column);
                }

                if (alreadyImported.Contains(moduleName))
                {
                    throw new AlreadyImportedException(
                        string.Format(Resources.Resources.ModuleAlreadyImported, moduleName),
                        _slangModule.ModuleName, importingModule.Symbol.Line, importingModule.Symbol.Column);
                }

                if (moduleName == _moduleRow.ModuleName)
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
            return base.VisitFuncDeclare(context);
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
            throw new KeywordCorrespondingException(
                string.Format(Resources.Resources.CorrespondingToKeywordError, id.GetText()), _slangModule.ModuleName,
                symbol.Line, symbol.Column);
        }
    }
}