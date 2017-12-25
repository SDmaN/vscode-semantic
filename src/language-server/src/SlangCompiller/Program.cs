using System;
using Microsoft.Extensions.CommandLineUtils;
using SlangCompiller.Resources;

namespace SlangCompiller
{
    internal class Program
    {
        public static void Main(string[] args = null)
        {
            CommandLineApplication application = new CommandLineApplication
            {
                Name = AppDomain.CurrentDomain.FriendlyName,
                Description = "Compiller for slang language."
            };

            application.HelpOption("-h|--help").Description = CommandLine.HelpDescription;

            application.Command("translate", TranslateCommand);

            application.Execute(args);
        }

        private static void TranslateCommand(CommandLineApplication c)
        {
            c.Description = CommandLine.Translate_Description;

            CommandArgument inputPathCommand = c.Argument("<inputPath>", CommandLine.Translate_InputDirectiory_Description);
            CommandArgument outputPathCommand = c.Argument("<outputPath>", CommandLine.Translate_OutputDirectory_Description);

            CommandOption help = c.HelpOption("-h|--help");
            help.Description = CommandLine.HelpDescription;

            c.OnExecute(() =>
            {
                string inputPath = inputPathCommand.Value;
                string outputPath = outputPathCommand.Value;

                if (string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath))
                {
                    c.ShowHelp();
                    return 0;
                }
                
                // TODO: compilation
                Console.WriteLine(inputPath);
                Console.WriteLine(outputPath);

                return 0;
            });
        }
    }
}