using System;
using System.Threading.Tasks;
using SlangGrammar;

namespace CompillerServices.Backend.Translators
{
    public interface ITranslator : IDisposable
    {
        Task Translate(SlangParser parser);
    }
}