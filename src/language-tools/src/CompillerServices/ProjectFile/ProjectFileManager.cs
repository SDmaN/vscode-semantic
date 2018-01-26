using System.IO;
using System.Linq;
using CompillerServices.Exceptions;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace CompillerServices.ProjectFile
{
    public class ProjectFileManager : IProjectFileManager
    {
        private const string ProjectFilePattern = "*.slproj";
        private readonly IStringLocalizer<ProjectFileManager> _localizer;

        public ProjectFileManager(IStringLocalizer<ProjectFileManager> localizer)
        {
            _localizer = localizer;
        }

        public FileInfo GetProjectFile(DirectoryInfo projectDirectory)
        {
            if (!projectDirectory.Exists)
            {
                throw new DirectoryNotFoundException(_localizer["Could not find directory {0}.",
                    projectDirectory.FullName]);
            }

            FileInfo[] files = projectDirectory.GetFiles(ProjectFilePattern);

            if (files.Length < 1)
            {
                throw new FileNotFoundException(_localizer["Project file does not exist in directory {0}.",
                    projectDirectory.FullName]);
            }

            if (files.Length > 1)
            {
                throw new ProjectFileException(_localizer["The directory must contain only one project file."]);
            }

            return files.First();
        }

        public string GetMainModule(DirectoryInfo projectDirectory)
        {
            FileInfo projectFile = GetProjectFile(projectDirectory);

            try
            {
                using (TextReader textReader = projectFile.OpenText())
                {
                    JsonReader jsonReader = new JsonTextReader(textReader);

                    JsonSerializer serializer = new JsonSerializer();
                    ProjectFileStructure structure = serializer.Deserialize<ProjectFileStructure>(jsonReader);

                    return structure.MainModule ??
                           throw new ProjectFileException(_localizer["Main module is not specified in project file."]);
                }
            }
            catch (JsonException)
            {
                throw new ProjectFileException(_localizer["Project file parsing error occurred."]);
            }
        }
    }
}