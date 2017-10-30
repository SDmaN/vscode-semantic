using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanguageServerProtocol.IPC
{
    public class StreamOutput : IOutput, IDisposable
    {
        private readonly Stream _stream;

        public StreamOutput(Stream stream)
        {
            _stream = stream;
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }

        public async Task WriteAsync(JToken response, CancellationToken cancellationToken = default)
        {
            TextWriter textWriter = new StreamWriter(_stream) { AutoFlush = true };
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);

            await response.WriteToAsync(jsonWriter, cancellationToken).ConfigureAwait(false);
        }
    }
}