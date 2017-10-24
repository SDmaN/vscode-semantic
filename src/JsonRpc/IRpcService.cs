using System.Threading;
using System.Threading.Tasks;

namespace JsonRpc
{
    public interface IRpcService
    {
        void RegisterHandler(object handler);
        Task HandleRequest(IInput input, IOutput output, CancellationToken cancellationToken = default);
    }
}