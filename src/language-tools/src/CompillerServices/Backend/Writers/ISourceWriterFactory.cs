using System.IO;

namespace CompillerServices.Backend.Writers
{
    public interface ISourceWriterFactory
    {
        ISourceWriter Create(FileInfo moduleFile, DirectoryInfo outputDirectory);
    }
}