using System.IO;
using System.Linq;
using CompillerServices.Exceptions;
using Newtonsoft.Json;

namespace CompillerServices.Backend.ProjectFile
{
    public class ProjectFileManager : IProjectFileManager
    {
        private const string ProjectFilePattern = "*.slproj";

        public string GetMainModule(DirectoryInfo projectDirectory)
        {
            FileInfo[] files = projectDirectory.GetFiles(ProjectFilePattern);

            if (files.Length < 1)
            {
                throw new FileNotFoundException(
                    string.Format(Strings.ProjectFileNotFound, projectDirectory.FullName));
            }

            if (files.Length > 1)
            {
                throw new ProjectFileException(string.Format(Strings.TooManyProjectFiles));
            }

            try
            {
                using (TextReader textReader = files.First().OpenText())
                {
                    JsonReader jsonReader = new JsonTextReader(textReader);

                    JsonSerializer serializer = new JsonSerializer();
                    ProjectFileStructure structure = serializer.Deserialize<ProjectFileStructure>(jsonReader);

                    return structure.MainModule ?? throw new ProjectFileException(Strings.MainModuleNotSpecified);
                }
            }
            catch (JsonException e)
            {
                throw new ProjectFileException(Strings.ProjectFileParsingException, e);
            }
        }
    }
}