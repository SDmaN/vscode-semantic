using System;
using System.IO;
using System.Threading.Tasks;
using CompillerServices.Backend;
using CompillerServices.DependencyInjection;
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

        public static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddCompillers();
                
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _localizer = _serviceProvider.GetService<IStringLocalizer<Program>>();
            _logger = _serviceProvider.GetService<ILogger<Program>>();

            InitializeCli(args);
        }

        private static void InitializeCli(string[] args)
        {
            CommandLineApplication application = new CommandLineApplication
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

            try
            {
                application.Execute(args);
            }
            catch (CommandParsingException)
            {
            }
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
                    return 0;
                }

                DirectoryInfo inputDirectory = new DirectoryInfo(inputPath);
                DirectoryInfo outputDirectory = new DirectoryInfo(outputPath);

                return await ProcessSources(inputDirectory,
                    async (sources, compiller) => await compiller.Translate(sources, outputDirectory));
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
                    return 0;
                }

                DirectoryInfo inputDirectory = new DirectoryInfo(inputPath);
                DirectoryInfo outputDirectory = new DirectoryInfo(outputPath);

                return await ProcessSources(inputDirectory,
                    async (sources, compiller) => await compiller.Build(sources, outputDirectory));
            });
        }

        private static async Task<int> ProcessSources(DirectoryInfo inputDirectory,
            Func<SourceContainer, IBackendCompiller, Task> processFunction)
        {
            try
            {
                IFileLoader fileLoader = _serviceProvider.GetService<IFileLoader>();
                SourceContainer sources = await fileLoader.LoadSources(inputDirectory);

                IFrontendCompiller frontendCompiller = _serviceProvider.GetService<IFrontendCompiller>();
                await frontendCompiller.CheckForErrors(sources);

                IBackendCompiller compiller = _serviceProvider.GetService<IBackendCompiller>();
                await processFunction(sources, compiller);
            }
            catch (Exception e)
            {
                _logger.LogCompillerError(e);
            }

            return 0;
        }
    }
}