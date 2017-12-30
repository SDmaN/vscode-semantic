using System.IO;

namespace CompillerServices.IO
{
    public class SlangModule
    {
        public SlangModule(string moduleName, FileInfo moduleFile, string content, bool isMain)
        {
            ModuleName = moduleName;
            ModuleFile = moduleFile;
            Content = content;
            IsMain = isMain;
        }

        public string ModuleName { get; }
        public FileInfo ModuleFile { get; }
        public string Content { get; }
        public bool IsMain { get; }
    }
}