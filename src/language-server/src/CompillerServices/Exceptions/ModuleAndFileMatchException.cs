using System;
using System.IO;

namespace CompillerServices.Exceptions
{
    public class ModuleAndFileMatchException : ApplicationException
    {
        public ModuleAndFileMatchException(FileInfo moduleFile, string moduleName)
            : base(string.Format(Resources.Resources.ModuleDoesNotMatchFile, moduleFile.Name, moduleName))
        {
        }
    }
}