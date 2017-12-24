using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanguageServerProtocol.IPC
{
    public class StreamOutput : IOutput, IDisposable
    {
        private const string CrLf = "\r\n";
        private const string ContentLengthHeader = "Content-Length: {0}" + CrLf;
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
            string responseString = response.ToString(Formatting.None);
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);
            byte[] responseHeaders = BuildHeaders(responseBytes);

            await _stream.WriteAsync(responseHeaders, 0, responseHeaders.Length, cancellationToken);
            await _stream.WriteAsync(responseBytes, 0, responseBytes.Length, cancellationToken);
            await _stream.FlushAsync(cancellationToken);
        }

        private static byte[] BuildHeaders(IReadOnlyCollection<byte> content)
        {
            StringBuilder headersBuilder = new StringBuilder();
            headersBuilder.AppendFormat(ContentLengthHeader, content.Count);
            headersBuilder.Append(CrLf);

            return Encoding.ASCII.GetBytes(headersBuilder.ToString());
        }
    }
}