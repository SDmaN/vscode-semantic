using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
            if(!slangModule.IsSystem)
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

        public async Task Build(SourceContainer sources, DirectoryInfo outputDirectory)
        {
            DirectoryInfo gccDirectory = new DirectoryInfo(Constants.CppCompillerPath);

            if (!gccDirectory.Exists)
            {
                throw new DirectoryNotFoundException(_localizer["Compiler directory {0} not exists.",
                    gccDirectory.FullName]);
            }

            await Translate(sources, outputDirectory);
            DirectoryInfo binDirectory = outputDirectory.CreateSubdirectory(Constants.CppOutput);

            _logger.LogCompillerBuilds(outputDirectory.FullName);

            string gccFileName = Path.Combine(gccDirectory.FullName, Constants.CppCompillerName);
            
            outputDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly);

            string gccArgs = await GetGccArgs(sources, outputDirectory);

            ProcessStartInfo processStartInfo = new ProcessStartInfo(gccFileName, gccArgs)
            {
                WorkingDirectory = gccDirectory.FullName
            };

            using (Process gppProcess = Process.Start(processStartInfo))
            {
                gppProcess.WaitForExit();

                if (gppProcess.ExitCode == 0)
                {
                    await PostBuild(gccDirectory, binDirectory);
                }
            }
        }

        private static Task<string> GetGccArgs(SourceContainer sources, DirectoryInfo outputDirectory)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"-o {Path.Combine(outputDirectory.FullName, Constants.CppOutput, sources.ProjectFile.GetShortNameWithoutExtension())}");
            builder.Append($" {Path.Combine(outputDirectory.FullName, "*" + Constants.CppSourceExtension)}");

            foreach (DirectoryInfo subDir in outputDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if(subDir.Name == Constants.CppOutput)
                {
                    continue;
                }

                builder.Append($" {Path.Combine(subDir.FullName, "*" + Constants.CppSourceExtension)}");
            }

            return Task.FromResult(builder.ToString());
        }

        private static Task PostBuild(DirectoryInfo gccDirectory, DirectoryInfo binDirectory)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IEnumerable<FileInfo> neededLibraries = gccDirectory.EnumerateFiles()
                    .Where(x => Constants.WindowsNeededLibraries.Contains(x.Name));

                foreach (FileInfo neededLibrary in neededLibraries)
                {
                    neededLibrary.CopyTo(Path.Combine(binDirectory.FullName, neededLibrary.Name));
                }
            }

            return Task.CompletedTask;
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