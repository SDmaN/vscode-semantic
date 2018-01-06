using System.IO;
using System.Threading.Tasks;
using CompillerServices.IO;

namespace CompillerServices.Backend
{
    public interface IBackendCompiller
    {
        Task Compile(SourceContainer sources, DirectoryInfo outputDirectory);
        Task Compile(SlangModule slangModule, DirectoryInfo outputDirectory);
        Task Build(SourceContainer sources, DirectoryInfo outputDirectory);
    }
}