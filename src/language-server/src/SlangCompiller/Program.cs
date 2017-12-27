using System;
using System.IO;
using CompillerServices.Backend;
using CompillerServices.DependencyInjection;
using CompillerServices.Frontend;
using CompillerServices.Output;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using SlangCompiller.Resources;

namespace SlangCompiller
{
    internal class Program
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
            var checker = ServiceProvider.GetService<IFrontendCompiller>();
            checker.CheckForErrors(new DirectoryInfo("C:\\Users\\sdman\\Desktop\\semlang")).Wait();

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

                IBackendCompiller compiller = ServiceProvider.GetService<IBackendCompiller>();

                await compiller.Compile(new DirectoryInfo(inputPath), new DirectoryInfo(outputPath),
                    (p, r) => Path.GetRelativePath(p.FullName, r.FullName));

                return 0;
            });
        }
    }
}