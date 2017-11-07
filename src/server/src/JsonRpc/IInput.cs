using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public interface IInput : IDisposable
    {
        Task<JToken> ReadAsync(CancellationToken cancellationToken = default);
    }
}