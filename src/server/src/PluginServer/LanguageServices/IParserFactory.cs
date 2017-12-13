using Antlr4.Runtime;
using SlangGrammar;

namespace PluginServer.LanguageServices
{
    public interface IParserFactory
    {
        SlangParser Create(Lexer lexer);
    }
}