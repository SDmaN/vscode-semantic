using Antlr4.Runtime;
using SlangGrammar;

namespace PluginServer.LanguageServices
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