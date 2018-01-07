using System.IO;

namespace CompillerServices.ProjectFile
{
    public interface IProjectFileManager
    {
        FileInfo GetProjectFile(DirectoryInfo projectDirectory);
        string GetMainModule(DirectoryInfo projectDirectory);
    }
}