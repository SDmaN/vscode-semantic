using System.IO;
using System.Threading.Tasks;

namespace CompillerServices.Backend.EntryPoint
{
    public interface IEntryPointWriter
    {
        Task WriteEntryPoint(DirectoryInfo inputDirectory, DirectoryInfo outputDirectory);
    }
}