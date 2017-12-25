using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using CompillerServices.Backend.EntryPoint;
using CompillerServices.Backend.Writers;
using CompillerServices.Output;
using SlangGrammar;
using SlangGrammar.Factories;
using RelativePathGetter = System.Func<System.IO.DirectoryInfo, System.IO.DirectoryInfo, string>;

namespace CompillerServices.Backend
{
    internal class ExceptionErrorListener : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine, string msg,
            RecognitionException e)
        {
            throw e;
        }
    }

    public class BackendCompiller : IBackendCompiller
    {
        private const string SlangFileMask = "*" + Constants.SlangExtension;
        private readonly IEntryPointWriter _entryPointWriter;
        private readonly ILexerFactory _lexerFactory;
        private readonly IOutputWriter _outputWriter;
        private readonly IParserFactory _parserFactory;
        private readonly ISourceWriterFactory _sourceWriterFactory;

        public BackendCompiller(ILexerFactory lexerFactory, IParserFactory parserFactory,
            ISourceWriterFactory sourceWriterFactory, IEntryPointWriter entryPointWriter, IOutputWriter outputWriter)
        {
            _lexerFactory = lexerFactory;
            _parserFactory = parserFactory;
            _sourceWriterFactory = sourceWriterFactory;
            _entryPointWriter = entryPointWriter;
            _outputWriter = outputWriter;
        }

        public async Task Compile(DirectoryInfo inputDirectory, DirectoryInfo outputDirectory,
            RelativePathGetter relativePathGetter)
        {
            if (!inputDirectory.Exists)
            {
                await _outputWriter.WriteError(string.Format(Strings.CouldNotFindDirectory, inputDirectory.FullName));
                return;
            }

            if (!outputDirectory.Exists)
            {
                outputDirectory.Create();
            }
            else
            {
                await ClearDirectory(outputDirectory);
            }

            try
            {
                await _entryPointWriter.WriteEntryPoint(inputDirectory, outputDirectory);
            }
            catch (Exception e)
            {
                await _outputWriter.WriteError(e.Message);
                return;
            }

            IEnumerable<FileInfo> inputFiles = inputDirectory.GetFiles(SlangFileMask, SearchOption.TopDirectoryOnly);

            foreach (FileInfo inputFile in inputFiles)
            {
                string relativePath = relativePathGetter(inputDirectory, inputFile.Directory);
                string outputPath = Path.Combine(outputDirectory.FullName, relativePath);

                await Compile(inputFile, outputPath);
            }
        }

        public async Task Compile(FileInfo inputFile, string outputPath)
        {
            await _outputWriter.WriteFileTranslating(inputFile);

            DirectoryInfo outputDirectory = new DirectoryInfo(outputPath);

            if (!outputDirectory.Exists)
            {
                outputDirectory.Create();
            }

            using (TextReader reader = inputFile.OpenText())
            {
                using (ISourceWriter sourceWriter = _sourceWriterFactory.Create(inputFile.Name, outputPath))
                {
                    string inputContent = await reader.ReadToEndAsync();

                    SlangLexer lexer = _lexerFactory.Create(inputContent);
                    SlangParser parser = _parserFactory.Create(lexer);
                    parser.AddErrorListener(new ExceptionErrorListener());

                    TranslatorVisitor visitor = new TranslatorVisitor(sourceWriter);

                    visitor.Visit(parser.start());
                    await sourceWriter.FlushAsync();
                }
            }
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