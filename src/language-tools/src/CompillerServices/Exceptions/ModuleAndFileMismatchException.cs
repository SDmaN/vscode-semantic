using System.IO;

namespace CompillerServices.Exceptions
{
    public class ModuleAndFileMismatchException : CompillerException
    {
        public ModuleAndFileMismatchException(FileInfo moduleFile, string moduleName, int line, int column)
            : base(string.Format(Resources.Resources.ModuleDoesNotMatchFile, moduleName, moduleFile.Name), moduleName, line, column)
        {
            ModuleFile = moduleFile;
        }

        public FileInfo ModuleFile { get; }
    }
}