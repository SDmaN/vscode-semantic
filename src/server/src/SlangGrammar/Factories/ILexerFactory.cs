namespace SlangGrammar.Factories
{
    public interface ILexerFactory
    {
        SlangLexer Create(string code);
    }
}