using System.IO;
using System.Threading.Tasks;

namespace CompillerServices.Output
{
    public interface IOutputWriter
    {
        Task WriteError(string errorMessage);
        Task WriteError(string errorMessage, int line, int symbol);
        Task WriteFileTranslating(FileInfo source);
        Task WriteDirectoryClean(DirectoryInfo cleainingDirectoryInfo);
    }
}