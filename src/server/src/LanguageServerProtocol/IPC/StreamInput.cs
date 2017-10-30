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

        private const char Cr = '\r';
        private const char Lf = '\n';
        private readonly Stream _inputStream;

        public StreamInput(Stream inputStream)
        {
            _inputStream = inputStream;
        }

        public void Dispose()
        {
            _inputStream?.Dispose();
        }

        public async Task<JToken> ReadAsync(CancellationToken cancellationToken = default)
        {
            IDictionary<string, string> headers = await ReadHeaders(cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            return await ReadBody(headers, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IDictionary<string, string>> ReadHeaders(CancellationToken cancellationToken = default)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            byte[] buffer = new byte[100];

            int readCount = _inputStream.Read(buffer, 0, 1);

            StringBuilder headerLineBuilder = new StringBuilder();

            while (readCount > 0 && !(buffer[0] == Cr && buffer[1] == Lf))
            {
                if (readCount >= 2 && buffer[readCount - 2] == Cr && buffer[readCount - 1] == Lf)
                {
                    string headerLinePart = Encoding.ASCII.GetString(buffer, 0, readCount - 2);
                    headerLineBuilder.Append(headerLinePart);

                    KeyValuePair<string, string> headerKeyValue = GetHeaderKeyValue(headerLineBuilder.ToString());
                    result.Add(headerKeyValue);

                    headerLineBuilder.Clear();

                    readCount = 0;
                }
                else if (readCount == buffer.Length)
                {
                    headerLineBuilder.Append(Encoding.ASCII.GetString(buffer));
                    readCount = 0;
                }

                readCount += await _inputStream.ReadAsync(buffer, readCount, 1, cancellationToken);
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

        private async Task<JToken> ReadBody(IDictionary<string, string> headers,
            CancellationToken cancellationToken = default)
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

            byte[] buffer = new byte[contentLength];
            int readCount = await _inputStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

            if (readCount != contentLength)
            {
                throw new InvalidOperationException("Invalid body length.");
            }

            string json = Encoding.UTF8.GetString(buffer);

            return JToken.Parse(json);
        }
    }
}