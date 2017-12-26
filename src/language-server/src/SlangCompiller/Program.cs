using System;
using System.Collections.Generic;
using System.IO;
using CompillerServices.Backend;
using CompillerServices.Backend.Writers;
using CompillerServices.DependencyInjection;
using Microsoft.Extensions.CommandLineUtils;
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
            InitializeCli(args);
        }

        private static void InitializeCli(string[] args)
        {
            CommandLineApplication application = new CommandLineApplication
            {
                Name = AppDomain.CurrentDomain.FriendlyName,
                Description = CommandLineStrings.ApplicationDescription
            };

            application.HelpOption("-h|--help").Description = CommandLineStrings.HelpDescription;

            application.Command("tr", TranslateCommand);

            application.OnExecute(() =>
            {
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
                string inputPath = inputPathCommand.Value;
                string outputPath = outputPathCommand.Value;

                if (string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath))
                {
                    c.ShowHelp();
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