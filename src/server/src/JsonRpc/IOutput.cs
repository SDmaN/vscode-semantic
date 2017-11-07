using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public interface IOutput : IDisposable
    {
        Task WriteAsync(JToken response, CancellationToken cancellationToken = default);
    }
}