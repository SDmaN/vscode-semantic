using System;
using System.IO;
using System.Threading.Tasks;

namespace CompillerServices.Output
{
    public abstract class StreamOutputWriter : IOutputWriter, IDisposable
    {
        private readonly Stream _outputStream;
        private readonly StreamWriter _outputWriter;

        protected StreamOutputWriter(Stream outputStream)
        {
            _outputStream = outputStream;
            _outputWriter = new StreamWriter(_outputStream);
        }

        public void Dispose()
        {
            _outputStream?.Dispose();
            _outputWriter?.Dispose();
        }

        public virtual async Task WriteFileTranslating(FileInfo source)
        {
            await WriteLineAsync($"[tr]: {source.Name}");
        }

        public virtual async Task WriteDirectoryClean(DirectoryInfo cleainingDirectoryInfo)
        {
            await WriteLineAsync($"[clean]: {cleainingDirectoryInfo.FullName}");
        }

        private async Task WriteLineAsync(string line)
        {
            await _outputWriter.WriteLineAsync(line);
            await _outputWriter.FlushAsync();
        }
    }
}