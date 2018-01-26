using System.IO;

namespace CompillerServices.Exceptions
{
    public class ModuleAndFileMismatchException : CompillerException
    {
        public ModuleAndFileMismatchException(string message, FileInfo moduleFile, string moduleName, int line,
            int column)
            : base(message, moduleName, line, column)
        {
            ModuleFile = moduleFile;
        }

        public FileInfo ModuleFile { get; }
    }
}