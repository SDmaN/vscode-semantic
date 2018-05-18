using System.IO;
using System.Threading.Tasks;
using CompillerServices.IO;

namespace CompillerServices.Cpp
{
    public interface ICppCompillerAdapter
    {
        Task Build(SourceContainer sources, DirectoryInfo outputDirectory);
    }
}
