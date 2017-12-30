using System.Threading.Tasks;
using CompillerServices.IO;

namespace CompillerServices.Frontend
{
    public interface IFrontendCompiller
    {
        Task CheckForErrors(SourceContainer sources);
    }
}