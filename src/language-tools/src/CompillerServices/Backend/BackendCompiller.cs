using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CompillerServices.Backend.EntryPoint;
using CompillerServices.Backend.TranslatorFactories;
using CompillerServices.Backend.Translators;
using CompillerServices.IO;
using CompillerServices.Output;
using SlangGrammar;
using SlangGrammar.Factories;

namespace CompillerServices.Backend
{
    public class BackendCompiller : IBackendCompiller
    {
        private const string CppOutput = "bin";

        private readonly IEntryPointWriter _entryPointWriter;
        private readonly ILexerFactory _lexerFactory;
        private readonly IOutputWriter _outputWriter;
        private readonly IParserFactory _parserFactory;
        private readonly ITranslatorFactory _translatorFactory;

        public BackendCompiller(ILexerFactory lexerFactory, IParserFactory parserFactory,
            ITranslatorFactory translatorFactory, IEntryPointWriter entryPointWriter, IOutputWriter outputWriter)
        {
            _lexerFactory = lexerFactory;
            _parserFactory = parserFactory;
            _translatorFactory = translatorFactory;
            _entryPointWriter = entryPointWriter;
            _outputWriter = outputWriter;
        }

        public async Task Compile(SourceContainer sources, DirectoryInfo outputDirectory)
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
                await Compile(slangModule, outputDirectory);
            }

            await _entryPointWriter.WriteEntryPoint(sources.MainModuleName, outputDirectory);
        }

        public async Task Compile(SlangModule slangModule, DirectoryInfo outputDirectory)
        {
            await _outputWriter.WriteFileTranslating(slangModule.ModuleFile);

            if (!outputDirectory.Exists)
            {
                outputDirectory.Create();
            }

            using (ITranslator translator = _translatorFactory.Create(slangModule.ModuleFile, outputDirectory))
            {
                SlangLexer lexer = _lexerFactory.Create(slangModule.Content);
                SlangParser parser = _parserFactory.Create(lexer);

                await translator.Translate(parser);
            }
        }

        public async Task Build(SourceContainer sources, DirectoryInfo outputDirectory)
        {
            await Compile(sources, outputDirectory);
            DirectoryInfo binDirectory = outputDirectory.CreateSubdirectory(CppOutput);
            DirectoryInfo gccDirectory = new DirectoryInfo(Constants.CppCompillerPath);

            if (!gccDirectory.Exists)
            {
                throw new DirectoryNotFoundException(string.Format(Resources.Resources.CompillerDirectoryNotFound,
                    gccDirectory.FullName));
            }

            string gccFileName = Path.Combine(gccDirectory.FullName, Constants.CppCompillerName);
            string gccArgs =
                $"-o {Path.Combine(outputDirectory.FullName, CppOutput, sources.MainModuleName)} {Path.Combine(outputDirectory.FullName, "*cpp")}";
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

        private async Task ClearDirectory(DirectoryInfo directory)
        {
            await _outputWriter.WriteDirectoryClean(directory);

            foreach (FileInfo file in directory.GetFiles().AsParallel())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories().AsParallel())
            {
                subDirectory.Delete(true);
            }
        }
    }
}