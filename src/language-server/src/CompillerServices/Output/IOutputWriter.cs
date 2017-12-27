using System.IO;
using System.Threading.Tasks;
using CompillerServices.Exceptions;

namespace CompillerServices.Output
{
    public interface IOutputWriter
    {
        Task WriteError(string errorMessage);
        Task WriteError(ErrorCheckException exception);
        Task WriteFileTranslating(FileInfo source);
        Task WriteDirectoryClean(DirectoryInfo cleainingDirectoryInfo);
    }
}