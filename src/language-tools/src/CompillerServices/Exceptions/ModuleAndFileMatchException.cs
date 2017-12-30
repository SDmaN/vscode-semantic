using System.IO;

namespace CompillerServices.Exceptions
{
    public class ModuleAndFileMatchException : ErrorCheckException
    {
        public ModuleAndFileMatchException(FileInfo moduleFile, string moduleName, int line, int column)
            : base(string.Format(Resources.Resources.ModuleDoesNotMatchFile, moduleName, moduleFile.Name), moduleName, line, column)
        {
            ModuleFile = moduleFile;
        }

        public FileInfo ModuleFile { get; }
    }
}