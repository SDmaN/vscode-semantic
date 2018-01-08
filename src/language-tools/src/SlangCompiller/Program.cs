using System;
using System.IO;
using System.Threading.Tasks;
using CompillerServices.Backend;
using CompillerServices.DependencyInjection;
using CompillerServices.Exceptions;
using CompillerServices.Frontend;
using CompillerServices.IO;
using CompillerServices.Output;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using SlangCompiller.Resources;

namespace SlangCompiller
{
    internal static class Program
    {
        private static IServiceProvider _serviceProvider;

        private static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider != null)
                {
                    return _serviceProvider;
                }

                IServiceCollection serviceCollection = new ServiceCollection();
                serviceCollection.AddCompillers();
                _serviceProvider = serviceCollection.BuildServiceProvider();

                return _serviceProvider;
            }
        }

        public static void Main(string[] args)
        {
            InitializeCli(args);
        }

        private static void InitializeCli(string[] args)
        {
            CommandLineApplication application = new CommandLineApplication
            {
                Name = AppDomain.CurrentDomain.FriendlyName,
                Description = CommandLineStrings.ApplicationDescription
            };

            CommandOption help = application.HelpOption("-h|--help");
            help.Description = CommandLineStrings.HelpDescription;

            application.Command("tr", TranslateCommand);
            application.Command("b", BuildCommand);

            application.OnExecute(() =>
            {
                if (help.HasValue())
                {
                    return 0;
                }

                Console.WriteLine(CommandLineStrings.Welcome);
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
            c.Description = CommandLineStrings.Translate_Description;

            CommandArgument inputPathCommand =
                c.Argument("<inputPath>", CommandLineStrings.Translate_InputDirectiory_Description);
            CommandArgument outputPathCommand =
                c.Argument("<outputPath>", CommandLineStrings.Translate_OutputDirectory_Description);

            CommandOption help = c.HelpOption("-h|--help");
            help.Description = CommandLineStrings.HelpDescription;

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

                IOutputWriter outputWriter = ServiceProvider.GetService<IOutputWriter>();

                if (isInputEmpty)
                {
                    await outputWriter.WriteError(string.Format(CommandLineStrings.PathNotSet, "<inputPath>"));
                }

                if (isOutputEmpty)
                {
                    await outputWriter.WriteError(string.Format(CommandLineStrings.PathNotSet, "<outputPath>"));
                }

                if (isOutputEmpty || isInputEmpty)
                {
                    return 0;
                }

                DirectoryInfo inputDirectory = new DirectoryInfo(inputPath);
                DirectoryInfo outputDirectory = new DirectoryInfo(outputPath);

                return await ProcessSources(outputWriter, inputDirectory,
                    async (sources, compiller) => await compiller.Translate(sources, outputDirectory));
            });
        }

        private static void BuildCommand(CommandLineApplication c)
        {
            c.Description = CommandLineStrings.Build_Description;

            CommandArgument inputPathCommand =
                c.Argument("<inputPath>", CommandLineStrings.Translate_InputDirectiory_Description);
            CommandArgument outputPathCommand =
                c.Argument("<outputPath>", CommandLineStrings.Translate_OutputDirectory_Description);

            CommandOption help = c.HelpOption("-h|--help");
            help.Description = CommandLineStrings.HelpDescription;

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

                IOutputWriter outputWriter = ServiceProvider.GetService<IOutputWriter>();

                if (isInputEmpty)
                {
                    await outputWriter.WriteError(string.Format(CommandLineStrings.PathNotSet, "<inputPath>"));
                }

                if (isOutputEmpty)
                {
                    await outputWriter.WriteError(string.Format(CommandLineStrings.PathNotSet, "<outputPath>"));
                }

                if (isOutputEmpty || isInputEmpty)
                {
                    return 0;
                }

                DirectoryInfo inputDirectory = new DirectoryInfo(inputPath);
                DirectoryInfo outputDirectory = new DirectoryInfo(outputPath);

                return await ProcessSources(outputWriter, inputDirectory,
                    async (sources, compiller) => await compiller.Build(sources, outputDirectory));
            });
        }

        private static async Task<int> ProcessSources(IOutputWriter outputWriter, DirectoryInfo inputDirectory,
            Func<SourceContainer, IBackendCompiller, Task> processFunction)
        {
            try
            {
                IFileLoader fileLoader = ServiceProvider.GetService<IFileLoader>();
                SourceContainer sources = await fileLoader.LoadSources(inputDirectory);

                IFrontendCompiller frontendCompiller = ServiceProvider.GetService<IFrontendCompiller>();
                await frontendCompiller.CheckForErrors(sources);

                IBackendCompiller compiller = ServiceProvider.GetService<IBackendCompiller>();
                await processFunction(sources, compiller);
            }
            catch (DirectoryNotFoundException e)
            {
                await outputWriter.WriteError(e.Message);
                return 1;
            }
            catch (FileNotFoundException e)
            {
                await outputWriter.WriteError(e.Message);
                return 1;
            }
            catch (ProjectFileException e)
            {
                await outputWriter.WriteError(e.Message);
                return 1;
            }
            catch (ModuleAndFileMatchException e)
            {
                await outputWriter.WriteError(e);
                return 1;
            }
            catch (CompillerException e)
            {
                await outputWriter.WriteError(e);
                return 1;
            }
            catch (Exception e)
            {
                await outputWriter.WriteError(string.Format(CommandLineStrings.UnknownError, e.Message));
                return 1;
            }

            return 0;
        }
    }
}