using System.IO;
using System.Threading.Tasks;
using CompillerServices.Exceptions;

namespace CompillerServices.Output
{
    public interface IOutputWriter
    {
        Task WriteError(string errorMessage);
        Task WriteError(CompillerException exception);
        Task WriteFileTranslating(FileInfo source);
        Task WriteDirectoryClean(DirectoryInfo cleainingDirectoryInfo);
        Task WriteBuilding(string message);
    }
}