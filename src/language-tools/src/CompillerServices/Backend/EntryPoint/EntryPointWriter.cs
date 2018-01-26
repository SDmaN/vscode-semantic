using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace CompillerServices.Backend.EntryPoint
{
    public class EntryPointWriter : IEntryPointWriter
    {
        private const string MainFile = "main-{0}.cpp";
        private readonly IStringLocalizer<EntryPointWriter> _localizer;

        public EntryPointWriter(IStringLocalizer<EntryPointWriter> localizer)
        {
            _localizer = localizer;
        }

        public async Task WriteEntryPoint(string mainModuleName, DirectoryInfo outputDirectory)
        {
            if (!outputDirectory.Exists)
            {
                outputDirectory.Create();
            }

            string mainFileName = string.Format(MainFile, Guid.NewGuid());

            using (FileStream entryFile = File.OpenWrite(Path.Combine(outputDirectory.FullName, mainFileName)))
            {
                TextWriter writer = new StreamWriter(entryFile);
                await writer.WriteAsync(_localizer["MainContent"].Value.Replace("###", mainModuleName));
                await writer.FlushAsync();
            }
        }
    }
}