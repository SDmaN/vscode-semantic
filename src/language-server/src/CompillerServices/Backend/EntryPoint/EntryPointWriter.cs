using System;
using System.IO;
using System.Threading.Tasks;
using CompillerServices.Backend.ProjectFile;
using CompillerServices.Resources;

namespace CompillerServices.Backend.EntryPoint
{
    public class EntryPointWriter : IEntryPointWriter
    {
        private const string MainFile = "main-{0}.cpp";
        private readonly IProjectFileManager _projectFileManager;

        public EntryPointWriter(IProjectFileManager projectFileManager)
        {
            _projectFileManager = projectFileManager;
        }

        public async Task WriteEntryPoint(DirectoryInfo inputDirectory, DirectoryInfo outputDirectory)
        {
            string mainModule = _projectFileManager.GetMainModule(inputDirectory);
            string mainFileName = string.Format(MainFile, Guid.NewGuid());

            using (FileStream entryFile = File.OpenWrite(Path.Combine(outputDirectory.FullName, mainFileName)))
            {
                TextWriter writer = new StreamWriter(entryFile);
                await writer.WriteAsync(Strings.MainContent.Replace("###", mainModule));
                await writer.FlushAsync();
            }
        }
    }
}