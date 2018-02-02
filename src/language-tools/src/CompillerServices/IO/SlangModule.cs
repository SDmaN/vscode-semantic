using System.IO;

namespace CompillerServices.IO
{
    public class SlangModule
    {
        public SlangModule(string moduleName, FileInfo moduleFile, string content, bool isMain, bool isSystem)
        {
            ModuleName = moduleName;
            ModuleFile = moduleFile;
            Content = content;
            IsMain = isMain;
            IsSystem = isSystem;
        }

        public string ModuleName { get; }
        public FileInfo ModuleFile { get; }
        public string Content { get; }
        public bool IsMain { get; }
        public bool IsSystem { get; }
    }
}