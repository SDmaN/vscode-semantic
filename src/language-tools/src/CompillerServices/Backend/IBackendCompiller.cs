using System.IO;
using System.Threading.Tasks;
using CompillerServices.IO;

namespace CompillerServices.Backend
{
    public interface IBackendCompiller
    {
        Task Translate(SourceContainer sources, DirectoryInfo outputDirectory);
        Task Translate(SourceContainer sources, SlangModule slangModule, DirectoryInfo outputDirectory);
        Task Build(SourceContainer sources, DirectoryInfo outputDirectory);
    }
}