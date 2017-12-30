using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CompillerServices.ProjectFile;

namespace CompillerServices.IO
{
    public class FileLoader : IFileLoader
    {
        private readonly IProjectFileManager _projectFileManager;

        public FileLoader(IProjectFileManager projectFileManager)
        {
            _projectFileManager = projectFileManager;
        }

        public async Task<SourceContainer> LoadSources(DirectoryInfo inputDirectory)
        {
            if (!inputDirectory.Exists)
            {
                throw new DirectoryNotFoundException(string.Format(Resources.Resources.CouldNotFindDirectory,
                    inputDirectory.FullName));
            }

            string mainModuleName = _projectFileManager.GetMainModule(inputDirectory);
            SourceContainer container = new SourceContainer
            {
                MainModuleName = mainModuleName
            };

            IEnumerable<FileInfo> inputFiles =
                inputDirectory.GetFiles(Constants.SlangFileMask, SearchOption.TopDirectoryOnly);

            foreach (FileInfo inputFile in inputFiles.AsParallel())
            {
                SlangModule slangModule = await LoadFileContent(inputFile, mainModuleName);
                container.Add(slangModule);
            }

            return container;
        }

        private static async Task<SlangModule> LoadFileContent(FileInfo file, string mainModuleName)
        {
            string moduleName = file.GetShortNameWithoutExtension();
            string content;

            using (TextReader fileReader = file.OpenText())
            {
                content = await fileReader.ReadToEndAsync();
            }

            return new SlangModule(moduleName, file, content, moduleName == mainModuleName);
        }
    }
}