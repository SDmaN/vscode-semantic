using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using LanguageServerProtocol.Exceptions;
using Newtonsoft.Json.Linq;

namespace LanguageServerProtocol.IPC
{
    public class StreamInput : IInput, IDisposable
    {
        private const char HeaderSeparator = ':';
        private const string ContentLengthHeader = "Content-Length";

        private readonly Stream _inputStream;

        public StreamInput(Stream inputStream)
        {
            _inputStream = new BufferedStream(inputStream);
        }

        public void Dispose()
        {
            _inputStream?.Dispose();
        }

        public async Task<JToken> ReadAsync(CancellationToken cancellationToken = default)
        {
            IDictionary<string, string> headers = await ReadHeaders(cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            return await ReadBody(headers);
        }

        public async Task<IDictionary<string, string>> ReadHeaders(CancellationToken cancellationToken = default)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            StreamReader reader = new StreamReader(_inputStream, Encoding.ASCII);

            string line = await reader.ReadLineAsync();

            while (!string.IsNullOrEmpty(line))
            {
                cancellationToken.ThrowIfCancellationRequested();
                result.Add(GetHeaderKeyValue(line));
                line = await reader.ReadLineAsync().ConfigureAwait(false);
            }

            return result;
        }

        private static KeyValuePair<string, string> GetHeaderKeyValue(string headerLine)
        {
            int separatorPos = headerLine.IndexOf(HeaderSeparator);

            if (separatorPos == -1)
            {
                throw new HeadersParsingException(headerLine, $"No separator found in line \"{headerLine}\".");
            }

            string key = headerLine.Substring(0, separatorPos).Trim();
            string value = headerLine.Substring(separatorPos + 1).Trim();

            return new KeyValuePair<string, string>(key, value);
        }

        private async Task<JToken> ReadBody(IDictionary<string, string> headers)
        {
            if (!headers.TryGetValue(ContentLengthHeader, out string value))
            {
                throw new InvalidHeaderException(ContentLengthHeader, $"{ContentLengthHeader} header not specified");
            }

            if (!long.TryParse(value, out long contentLength))
            {
                throw new InvalidHeaderException(ContentLengthHeader,
                    $"{ContentLengthHeader} has invalid value ({value}).");
            }

            StreamReader reader = new StreamReader(_inputStream);
            char[] buffer = new char[contentLength];

            int readCount = await reader.ReadBlockAsync(buffer, 0, buffer.Length);

            if (readCount != contentLength)
            {
                throw new BodyLengthException($"Invalid body length ({readCount} except of {contentLength}).");
            }

            return JToken.Parse(new string(buffer));
        }
    }
}