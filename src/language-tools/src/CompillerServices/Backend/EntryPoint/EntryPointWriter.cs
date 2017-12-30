using System;
using System.IO;
using System.Threading.Tasks;

namespace CompillerServices.Backend.EntryPoint
{
    public class EntryPointWriter : IEntryPointWriter
    {
        private const string MainFile = "main-{0}.cpp";

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
                await writer.WriteAsync(Resources.Resources.MainContent.Replace("###", mainModuleName));
                await writer.FlushAsync();
            }
        }
    }
}