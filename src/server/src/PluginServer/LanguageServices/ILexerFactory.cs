using SlangGrammar;

namespace PluginServer.LanguageServices
{
    public interface ILexerFactory
    {
        SlangLexer Create(string code);
    }
}