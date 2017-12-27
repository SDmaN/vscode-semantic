using System.IO;
using System.Threading.Tasks;

namespace CompillerServices.Frontend
{
    public interface IFrontendCompiller
    {
        Task CheckForErrors(DirectoryInfo inputDirectory);
    }
}