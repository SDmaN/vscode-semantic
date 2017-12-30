using Antlr4.Runtime;

namespace SlangGrammar.Factories
{
    public interface IParserFactory
    {
        SlangParser Create(Lexer lexer);
    }
}