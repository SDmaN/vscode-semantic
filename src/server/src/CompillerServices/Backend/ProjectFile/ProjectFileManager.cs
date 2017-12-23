using System.IO;
using System.Linq;
using CompillerServices.Exceptions;
using Newtonsoft.Json;

namespace CompillerServices.Backend.ProjectFile
{
    public class ProjectFileManager : IProjectFileManager
    {
        private const string ProjectFilePattern = "*.slang";

        public string GetMainModule(DirectoryInfo projectDirectory)
        {
            FileInfo[] files = projectDirectory.GetFiles(ProjectFilePattern);

            if (files.Length < 1)
            {
                throw new FileNotFoundException(
                    $"Project file does not exists in directory {projectDirectory.FullName}.");
            }

            if (files.Length > 1)
            {
                throw new ProjectFileException(
                    "More than one project file found. The directory must contain only one project file.");
            }

            using (TextReader textReader = files.First().OpenText())
            {
                JsonReader jsonReader = new JsonTextReader(textReader);

                JsonSerializer serializer = new JsonSerializer();
                ProjectFileStructure structure = serializer.Deserialize<ProjectFileStructure>(jsonReader);

                return structure.MainModule;
            }
        }
    }
}