using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CompillerServices.Backend.EntryPoint;
using CompillerServices.Backend.Translators;
using CompillerServices.IO;
using CompillerServices.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SlangGrammar;
using SlangGrammar.Factories;

namespace CompillerServices.Backend
{
    public class BackendCompiller : IBackendCompiller
    {
        private readonly ILexerFactory _lexerFactory;
        private readonly IParserFactory _parserFactory;
        private readonly ITranslatorFactory _translatorFactory;
        private readonly IEntryPointWriter _entryPointWriter;
        private readonly ILogger<BackendCompiller> _logger;
        private readonly IStringLocalizer<BackendCompiller> _localizer;

        public BackendCompiller(ILexerFactory lexerFactory, IParserFactory parserFactory,
            ITranslatorFactory translatorFactory, IEntryPointWriter entryPointWriter, ILogger<BackendCompiller> logger,
            IStringLocalizer<BackendCompiller> localizer)
        {
            _lexerFactory = lexerFactory;
            _parserFactory = parserFactory;
            _translatorFactory = translatorFactory;
            _entryPointWriter = entryPointWriter;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task Translate(SourceContainer sources, DirectoryInfo outputDirectory)
        {
            if (!outputDirectory.Exists)
            {
                outputDirectory.Create();
            }
            else
            {
                await ClearDirectory(outputDirectory);
            }

            foreach (SlangModule slangModule in sources)
            {
                if (!slangModule.IsSystem)
                {
                    await Translate(sources, slangModule, outputDirectory);
                }
                else
                {
                    await Translate(sources, slangModule, new DirectoryInfo(Path.Combine(outputDirectory.FullName, "System")));
                }
            }

            await _entryPointWriter.WriteEntryPoint(sources.MainModuleName, outputDirectory);
        }

        public async Task Translate(SourceContainer sources, SlangModule slangModule, DirectoryInfo outputDirectory)
        {
            if (!slangModule.IsSystem)
            {
                _logger.LogCompillerTranslates(slangModule.ModuleName);
            }

            if (!outputDirectory.Exists)
            {
                outputDirectory.Create();
            }

            using (ITranslator translator = _translatorFactory.Create(slangModule.ModuleFile, outputDirectory, sources))
            {
                SlangLexer lexer = _lexerFactory.Create(slangModule.Content);
                SlangParser parser = _parserFactory.Create(lexer);

                await translator.Translate(parser);
            }
        }

        private Task ClearDirectory(DirectoryInfo directory)
        {
            _logger.LogCompillerCleans(directory.FullName);

            foreach (FileInfo file in directory.GetFiles().AsParallel())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories().AsParallel())
            {
                subDirectory.Delete(true);
            }

            return Task.CompletedTask;
        }
    }
}