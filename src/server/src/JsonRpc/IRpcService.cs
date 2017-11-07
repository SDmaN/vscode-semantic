using System.Threading;
using System.Threading.Tasks;

namespace JsonRpc
{
    public interface IRpcService
    {
        Task HandleRequest(CancellationToken cancellationToken = default);
    }
}