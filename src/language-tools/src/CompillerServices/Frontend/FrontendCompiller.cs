using System.IO;
using System.Threading.Tasks;
using CompillerServices.Frontend.NameTables;
using CompillerServices.IO;
using Microsoft.Extensions.Localization;
using SlangGrammar;
using SlangGrammar.Factories;

namespace CompillerServices.Frontend
{
    internal class FrontendCompiller : IFrontendCompiller
    {
        private readonly ILexerFactory _lexerFactory;
        private readonly IStringLocalizer<FrontendCompiller> _localizer;
        private readonly INameTableContainer _nameTableContainer;
        private readonly IParserFactory _parserFactory;
        private readonly IStepVisitorFactory _stepVisitorFactory;

        public FrontendCompiller(IStringLocalizer<FrontendCompiller> localizer, INameTableContainer nameTableContainer,
            ILexerFactory lexerFactory, IParserFactory parserFactory, IStepVisitorFactory stepVisitorFactory)
        {
            _localizer = localizer;
            _nameTableContainer = nameTableContainer;
            _lexerFactory = lexerFactory;
            _parserFactory = parserFactory;
            _stepVisitorFactory = stepVisitorFactory;
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

        private void CheckMainModuleExists(SourceContainer sourceContainer)
        {
            if (sourceContainer.ContainsMainModule)
            {
                return;
            }

            string mainModuleFileName = $"{sourceContainer.MainModuleName}{Constants.SlangExtension}";
            throw new FileNotFoundException(_localizer["Main module file {0} not found.", mainModuleFileName]);
        }

        private async Task FirstStep(SlangParser parser, SlangModule slangModule)
        {
            FirstStepVisitor visitor = _stepVisitorFactory.CreateFirstStepVisitor(_nameTableContainer, slangModule);
            await Task.Run(() => visitor.Visit(parser.start()));
        }

        private async Task SecondStep(SlangParser parser, SlangModule slangModule)
        {
            SecondStepVisitor visitor = _stepVisitorFactory.CreateSecondStepVisitor(_nameTableContainer, slangModule);
            await Task.Run(() => visitor.Visit(parser.start()));
        }
    }
}