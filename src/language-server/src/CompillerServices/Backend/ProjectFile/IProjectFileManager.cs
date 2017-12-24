using System.IO;

namespace CompillerServices.Backend.ProjectFile
{
    public interface IProjectFileManager
    {
        string GetMainModule(DirectoryInfo projectDirectory);
    }
}