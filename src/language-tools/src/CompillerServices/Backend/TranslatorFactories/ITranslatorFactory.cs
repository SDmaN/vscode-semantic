using System.IO;
using CompillerServices.Backend.Translators;

namespace CompillerServices.Backend.TranslatorFactories
{
    public interface ITranslatorFactory
    {
        ITranslator Create(FileInfo moduleFile, DirectoryInfo outputDirectory);
    }
}