using Antlr4.Runtime;

namespace SlangGrammar.Factories
{
    public class ParserFactory : IParserFactory
    {
        public SlangParser Create(Lexer lexer)
        {
            ITokenStream tokenStream = new CommonTokenStream(lexer);
            return new SlangParser(tokenStream);
        }
    }
}