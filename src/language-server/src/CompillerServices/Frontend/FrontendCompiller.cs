using System;
using System.IO;
using System.Threading.Tasks;
using Antlr4.Runtime;
using CompillerServices.Frontend.NameTables;
using CompillerServices.IO;
using SlangGrammar;
using SlangGrammar.Factories;

namespace CompillerServices.Frontend
{
    public class FrontendCompiller : IFrontendCompiller
    {
        private readonly ILexerFactory _lexerFactory;
        private readonly INameTableContainer _nameTableContainer;
        private readonly IParserFactory _parserFactory;

        public FrontendCompiller(INameTableContainer nameTableContainer, ILexerFactory lexerFactory,
            IParserFactory parserFactory)
        {
            _nameTableContainer = nameTableContainer;
            _lexerFactory = lexerFactory;
            _parserFactory = parserFactory;
        }

        public async Task CheckForErrors(SourceContainer sources)
        {
            CheckMainModuleExists(sources);
            await _nameTableContainer.Clear();

            foreach (SlangModule slangModule in sources)
            {
                await FirstStep(slangModule);
            }
        }

        private static void CheckMainModuleExists(SourceContainer sourceContainer)
        {
            if (sourceContainer.ContainsMainModule)
            {
                return;
            }

            string mainModuleFileName = $"{sourceContainer.MainModuleName}{Constants.SlangExtension}";
            throw new FileNotFoundException(string.Format(Resources.Resources.MainModuleFileNotFound,
                mainModuleFileName));
        }

        private async Task FirstStep(SlangModule slangModule)
        {
            SlangLexer lexer = _lexerFactory.Create(slangModule.Content);
            SlangParser parser = _parserFactory.Create(lexer);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ExceptionErrorListener());

            FirstStepVisitor visitor = new FirstStepVisitor(_nameTableContainer, slangModule);
            await Task.Run(() => visitor.Visit(parser.start()));
        }
    }

    internal class ExceptionErrorListener : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine, string msg,
            RecognitionException e)
        {
            if (e != null)
            {
                throw e;
            }

            throw new InvalidOperationException(msg);
        }
    }
}