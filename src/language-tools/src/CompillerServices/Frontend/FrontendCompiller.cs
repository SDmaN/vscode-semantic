using System;
using System.IO;
using System.Threading.Tasks;
using Antlr4.Runtime;
using CompillerServices.Exceptions;
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
                SlangLexer lexer = _lexerFactory.Create(slangModule.Content);
                SlangParser parser = _parserFactory.Create(lexer);
                parser.AddErrorListener(new ExceptionErrorListener(slangModule));

                await FirstStep(parser, slangModule);
            }

            foreach (SlangModule slangModule in sources)
            {
                SlangLexer lexer = _lexerFactory.Create(slangModule.Content);
                SlangParser parser = _parserFactory.Create(lexer);
                parser.AddErrorListener(new ExceptionErrorListener(slangModule));

                await SecondStep(parser, slangModule);
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

        private async Task FirstStep(SlangParser parser, SlangModule slangModule)
        {
            FirstStepVisitor visitor = new FirstStepVisitor(_nameTableContainer, slangModule);
            await Task.Run(() => visitor.Visit(parser.start()));
        }

        private async Task SecondStep(SlangParser parser, SlangModule slangModule)
        {
            /*SecondStepVisitor visitor = new SecondStepVisitor(_nameTableContainer, slangModule);
            await Task.Run(() => visitor.Visit(parser.start()));*/
        }
    }

    internal class ExceptionErrorListener : BaseErrorListener
    {
        private readonly SlangModule _module;

        public ExceptionErrorListener(SlangModule module)
        {
            _module = module;
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine, string msg, RecognitionException e)
        {
            throw new CompillerException(msg, _module.ModuleName, line, charPositionInLine);
        }
    }
}