using System.Threading;
using System.Threading.Tasks;

namespace JsonRpc
{
    public interface IRpcService
    {
        Task HandleRequest(IInput input, IOutput output, CancellationToken cancellationToken = default);
    }
}