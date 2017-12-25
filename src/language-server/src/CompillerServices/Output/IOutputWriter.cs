using System.IO;
using System.Threading.Tasks;

namespace CompillerServices.Output
{
    public interface IOutputWriter
    {
        Task WriteError(string errorMessage);
        Task WriteFileTranslating(FileInfo source);
        Task WriteDirectoryClean(DirectoryInfo cleainingDirectoryInfo);
    }
}