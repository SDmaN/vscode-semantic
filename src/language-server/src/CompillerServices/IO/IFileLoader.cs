using System.IO;
using System.Threading.Tasks;

namespace CompillerServices.IO
{
    public interface IFileLoader
    {
        Task<SourceContainer> LoadSources(DirectoryInfo inputDirectory);
    }
}