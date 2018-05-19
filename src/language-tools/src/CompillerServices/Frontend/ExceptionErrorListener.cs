using Antlr4.Runtime;
using CompillerServices.Exceptions;
using CompillerServices.IO;

namespace CompillerServices.Frontend
{
    internal class ExceptionErrorListener : BaseErrorListener
    {
        private readonly SlangModule _module;

        public ExceptionErrorListener(SlangModule module)
        {
            _module = module;
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine, string msg, RecognitionException e)
        {
            throw new CompillerException(msg, _module.ModuleFile, line, charPositionInLine);
        }
    }
}