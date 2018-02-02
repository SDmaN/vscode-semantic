using System.IO;
using CompillerServices.IO;

namespace CompillerServices.Backend.Translators
{
    public interface ITranslatorFactory
    {
        ITranslator Create(FileInfo moduleFile, DirectoryInfo outputDirectory, SourceContainer sourceContainer);
    }
}