using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public interface IInput
    {
        Task<JToken> ReadAsync(CancellationToken cancellationToken = default);
    }
}