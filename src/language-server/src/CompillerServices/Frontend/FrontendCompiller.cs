using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Antlr4.Runtime;
using CompillerServices.Frontend.NameTables;
using CompillerServices.ProjectFile;
using SlangGrammar;
using SlangGrammar.Factories;

namespace CompillerServices.Frontend
{
    internal class ExceptionErrorListener : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            if (e != null)
            {
                throw e;
            }

            throw new InvalidOperationException(msg);
        }
    }

    public class FrontendCompiller : IFrontendCompiller
    {
        private readonly ILexerFactory _lexerFactory;
        private readonly INameTableContainer _nameTableContainer;
        private readonly IParserFactory _parserFactory;
        private readonly IProjectFileManager _projectFileManager;

        public FrontendCompiller(INameTableContainer nameTableContainer, ILexerFactory lexerFactory,
            IParserFactory parserFactory, IProjectFileManager projectFileManager)
        {
            _nameTableContainer = nameTableContainer;
            _lexerFactory = lexerFactory;
            _parserFactory = parserFactory;
            _projectFileManager = projectFileManager;
        }

        public async Task CheckForErrors(DirectoryInfo inputDirectory)
        {
            if (!inputDirectory.Exists)
            {
                throw new DirectoryNotFoundException(string.Format(Resources.Resources.CouldNotFindDirectory,
                    inputDirectory.FullName));
            }

            await _nameTableContainer.Clear();
            _projectFileManager.GetMainModuleFile(inputDirectory);

            IEnumerable<FileInfo> inputFiles =
                inputDirectory.GetFiles(Constants.SlangFileMask, SearchOption.TopDirectoryOnly);

            foreach (FileInfo inputFile in inputFiles)
            {
                await FirstStep(inputFile);
            }
        }

        private async Task FirstStep(FileInfo file)
        {
            using (TextReader reader = file.OpenText())
            {
                string inputContent = await reader.ReadToEndAsync();

                SlangLexer lexer = _lexerFactory.Create(inputContent);
                SlangParser parser = _parserFactory.Create(lexer);
                parser.RemoveErrorListeners();
                parser.AddErrorListener(new ExceptionErrorListener());

                FirstStepVisitor visitor = new FirstStepVisitor(file, _nameTableContainer);
                visitor.Visit(parser.start());
            }
        }
    }
}