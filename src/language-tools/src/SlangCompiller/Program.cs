using System;
using System.IO;
using System.Threading.Tasks;
using CompillerServices.Backend;
using CompillerServices.Cpp;
using CompillerServices.Frontend;
using CompillerServices.IO;
using CompillerServices.Logging;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace SlangCompiller
{
    internal class Program
    {
        private static IServiceProvider _serviceProvider;
        private static IStringLocalizer<Program> _localizer;
        private static ILogger<Program> _logger;

        public static int Main(string[] args)
        {
            var startup = new Startup();
            _serviceProvider = startup.ConfigureServices();

            _localizer = _serviceProvider.GetService<IStringLocalizer<Program>>();
            _logger = _serviceProvider.GetService<ILogger<Program>>();

            try
            {
                return InitializeCli().Execute(args);
            }
            catch(CommandParsingException)
            {
            }

            return 0;
        }

        private static CommandLineApplication InitializeCli()
        {
            var application = new CommandLineApplication
            {
                Name = AppDomain.CurrentDomain.FriendlyName,
                Description = _localizer["Compiller for Slang language."]
            };

            CommandOption help = application.HelpOption("-h|--help");
            help.Description = _localizer["Show help."];

            application.Command("tr", TranslateCommand);
            application.Command("b", BuildCommand);

            application.OnExecute(() =>
            {
                if (help.HasValue())
                {
                    return 0;
                }

                Console.WriteLine(_localizer["Welcome to Slang to C++ translator and compiller!"]);
                application.ShowHint();

                return 0;
            });

            return application;
        }

        private static void TranslateCommand(CommandLineApplication c)
        {
            c.Description = _localizer["Translates Slang code to C++ code."];

            CommandArgument inputPathCommand =
                c.Argument("<inputPath>", _localizer["Directory that contains source Slang code."]);
            CommandArgument outputPathCommand =
                c.Argument("<outputPath>", _localizer["Directory where output C++ code will be stored."]);

            CommandOption help = c.HelpOption("-h|--help");
            help.Description = _localizer["Show help."];

            c.OnExecute(async () =>
            {
                if (help.HasValue())
                {
                    return 0;
                }

                string inputPath = inputPathCommand.Value;
                string outputPath = outputPathCommand.Value;

                bool isInputEmpty = string.IsNullOrEmpty(inputPath);
                bool isOutputEmpty = string.IsNullOrWhiteSpace(outputPath);

                if (isInputEmpty)
                {
                    _logger.LogCompillerError(_localizer["Path not specified: {0}.", "<inputPath>"]);
                }

                if (isOutputEmpty)
                {
                    _logger.LogCompillerError(_localizer["Path not specified: {0}.", "<outputPath>"]);
                }

                if (isOutputEmpty || isInputEmpty)
                {
                    return 1;
                }

                var inputDirectory = new DirectoryInfo(inputPath);
                var outputDirectory = new DirectoryInfo(outputPath);

                return await ProcessSources(inputDirectory,
                    async (sources, compiller, adapter) => await compiller.Translate(sources, outputDirectory));
            });
        }

        private static void BuildCommand(CommandLineApplication c)
        {
            c.Description = "Translates Slang code to C++ code and builds an executable.";

            CommandArgument inputPathCommand =
                c.Argument("<inputPath>", _localizer["Directory that contains source Slang code."]);
            CommandArgument outputPathCommand =
                c.Argument("<outputPath>", _localizer["Directory where output C++ code will be stored."]);

            CommandOption help = c.HelpOption("-h|--help");
            help.Description = _localizer["Show help."];

            c.OnExecute(async () =>
            {
                if (help.HasValue())
                {
                    return 0;
                }

                string inputPath = inputPathCommand.Value;
                string outputPath = outputPathCommand.Value;

                bool isInputEmpty = string.IsNullOrEmpty(inputPath);
                bool isOutputEmpty = string.IsNullOrWhiteSpace(outputPath);

                if (isInputEmpty)
                {
                    _logger.LogCompillerError(_localizer["Path not specified: {0}.", "<inputPath>"]);
                }

                if (isOutputEmpty)
                {
                    _logger.LogCompillerError(_localizer["Path not specified: {0}.", "<outputPath>"]);
                }

                if (isOutputEmpty || isInputEmpty)
                {
                    return 1;
                }

                var inputDirectory = new DirectoryInfo(inputPath);
                var outputDirectory = new DirectoryInfo(outputPath);

                return await ProcessSources(inputDirectory,
                    async (sources, compiller, adapter) =>
                    {
                        await compiller.Translate(sources, outputDirectory);
                        await adapter.Build(sources, outputDirectory);
                    });
            });
        }

        private static async Task<int> ProcessSources(DirectoryInfo inputDirectory,
            Func<SourceContainer, IBackendCompiller, ICppCompillerAdapter, Task> processFunction)
        {
            try
            {
                var fileLoader = _serviceProvider.GetService<IFileLoader>();
                SourceContainer sources = await fileLoader.LoadSources(inputDirectory);

                var frontendCompiller = _serviceProvider.GetService<IFrontendCompiller>();
                await frontendCompiller.CheckForErrors(sources);

                var compiller = _serviceProvider.GetService<IBackendCompiller>();
                var adapter = _serviceProvider.GetService<ICppCompillerAdapter>();
                await processFunction(sources, compiller, adapter);
            }
            catch (Exception e)
            {
                _logger.LogCompillerError(e);
                return 1;
            }

            return 0;
        }
    }
}