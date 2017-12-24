using System;
using System.IO;
using System.Threading.Tasks;

namespace CompillerServices.Backend
{
    public interface IBackendCompiller
    {
        Task Compile(DirectoryInfo inputDirectory, DirectoryInfo outputDirectory,
            Func<DirectoryInfo, DirectoryInfo, string> relativePathGetter);

        Task Compile(FileInfo inputFile, string outputPath);
    }
}