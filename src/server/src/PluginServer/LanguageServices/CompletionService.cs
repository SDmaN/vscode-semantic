using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.Handlers.TextDocument.Completion;
using SlangGrammar;

namespace PluginServer.LanguageServices
{
    internal class CompletionVisitor : SlangBaseVisitor<IEnumerable<CompletionItem>>
    {
        private readonly IList<CompletionItem> _completions = new List<CompletionItem>();
        private readonly Position _position;

        public CompletionVisitor(Position position)
        {
            _position = position;
        }

        public override IEnumerable<CompletionItem> VisitStart(SlangParser.StartContext context)
        {
            SlangParser.ModuleContext module = context.module();

            // Модуль еще не объявлен
            if (module.ChildCount == 0)
            {
            }

            SlangParser.ModuleImportsContext imports = context.moduleImports();

            return _completions;
        }

        public override IEnumerable<CompletionItem> VisitModuleImports(SlangParser.ModuleImportsContext context)
        {
            return _completions;
        }
    }

    public interface ICompletionService
    {
        IEnumerable<CompletionItem> GetCompletionItems(TextDocumentIdentifier documentIdentifier,
            Position position);
    }

    public class CompletionService : ICompletionService
    {
        private readonly IFileContentKeeper _contentKeeper;
        private readonly ILexerFactory _lexerFactory;
        private readonly IParserFactory _parserFactory;

        public CompletionService(IFileContentKeeper contentKeeper, ILexerFactory lexerFactory,
            IParserFactory parserFactory)
        {
            _contentKeeper = contentKeeper;
            _lexerFactory = lexerFactory;
            _parserFactory = parserFactory;
        }

        public IEnumerable<CompletionItem> GetCompletionItems(TextDocumentIdentifier documentIdentifier,
            Position position)
        {
            string content = _contentKeeper.Get(documentIdentifier.Uri);
            SlangLexer lexer = _lexerFactory.Create(content);
            SlangParser parser = _parserFactory.Create(lexer);
            parser.AddErrorListener(new MyErrorListener());

            CompletionVisitor visitor = new CompletionVisitor(position);
            IEnumerable<CompletionItem> completions = visitor.Visit(parser.start());

            return completions ?? Enumerable.Empty<CompletionItem>();
        }

        private class MyErrorListener : BaseErrorListener
        {
            public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
                int charPositionInLine, string msg,
                RecognitionException e)
            {
                var a = recognizer.Atn.GetExpectedTokens(e.OffendingState, e.Context);
            }
        }
    }
}