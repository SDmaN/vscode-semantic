using Antlr4.Runtime;
using SlangGrammar;

namespace PluginServer.LanguageServices
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