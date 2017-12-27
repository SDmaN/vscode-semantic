using System.IO;
using System.Threading.Tasks;

namespace CompillerServices.Backend.EntryPoint
{
    public interface IEntryPointWriter
    {
        Task WriteEntryPoint(string mainModuleName, DirectoryInfo outputDirectory);
    }
}