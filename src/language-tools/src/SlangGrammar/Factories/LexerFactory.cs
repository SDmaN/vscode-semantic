using Antlr4.Runtime;

namespace SlangGrammar.Factories
{
    public class LexerFactory : ILexerFactory
    {
        public SlangLexer Create(string code)
        {
            AntlrInputStream inputStream = new AntlrInputStream(code);
            return new SlangLexer(inputStream);
        }
    }
}