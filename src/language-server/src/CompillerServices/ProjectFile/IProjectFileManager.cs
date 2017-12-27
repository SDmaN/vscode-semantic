using System.IO;

namespace CompillerServices.ProjectFile
{
    public interface IProjectFileManager
    {
        string GetMainModule(DirectoryInfo projectDirectory);
        FileInfo GetMainModuleFile(DirectoryInfo projectDirectory);
    }
}