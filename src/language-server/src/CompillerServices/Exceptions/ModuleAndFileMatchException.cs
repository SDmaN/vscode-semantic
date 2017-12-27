using System.IO;

namespace CompillerServices.Exceptions
{
    public class ModuleAndFileMatchException : ErrorCheckException
    {
        public ModuleAndFileMatchException(FileInfo moduleFile, string moduleName, int line, int column)
            : base(string.Format(Resources.Resources.ModuleDoesNotMatchFile, moduleName, moduleFile.Name), line, column)
        {
            ModuleFile = moduleFile;
            ModuleName = moduleName;
        }

        public FileInfo ModuleFile { get; }
        public string ModuleName { get; }
    }
}