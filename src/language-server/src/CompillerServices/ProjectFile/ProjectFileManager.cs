using System.IO;
using System.Linq;
using CompillerServices.Exceptions;
using Newtonsoft.Json;

namespace CompillerServices.ProjectFile
{
    public class ProjectFileManager : IProjectFileManager
    {
        private const string ProjectFilePattern = "*.slproj";

        public string GetMainModule(DirectoryInfo projectDirectory)
        {
            if (!projectDirectory.Exists)
            {
                throw new DirectoryNotFoundException(string.Format(Resources.Resources.CouldNotFindDirectory,
                    projectDirectory.FullName));
            }

            FileInfo[] files = projectDirectory.GetFiles(ProjectFilePattern);

            if (files.Length < 1)
            {
                throw new FileNotFoundException(
                    string.Format(Resources.Resources.ProjectFileNotFound, projectDirectory.FullName));
            }

            if (files.Length > 1)
            {
                throw new ProjectFileException(string.Format(Resources.Resources.TooManyProjectFiles));
            }

            try
            {
                using (TextReader textReader = files.First().OpenText())
                {
                    JsonReader jsonReader = new JsonTextReader(textReader);

                    JsonSerializer serializer = new JsonSerializer();
                    ProjectFileStructure structure = serializer.Deserialize<ProjectFileStructure>(jsonReader);

                    return structure.MainModule ??
                           throw new ProjectFileException(Resources.Resources.MainModuleNotSpecified);
                }
            }
            catch (JsonException e)
            {
                throw new ProjectFileException(Resources.Resources.ProjectFileParsingException, e);
            }
        }

        public FileInfo GetMainModuleFile(DirectoryInfo projectDirectory)
        {
            string mainModuleName = GetMainModule(projectDirectory);
            string mainModuleFileName = $"{mainModuleName}{Constants.SlangExtension}";

            FileInfo moduleFile = projectDirectory.EnumerateFiles(mainModuleFileName)
                .FirstOrDefault();

            if (moduleFile == null)
            {
                throw new FileNotFoundException(string.Format(Resources.Resources.MainModuleFileNotFound,
                    mainModuleFileName));
            }

            return moduleFile;
        }
    }
}