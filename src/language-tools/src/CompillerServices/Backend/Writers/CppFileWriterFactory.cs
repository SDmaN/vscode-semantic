using System.IO;

namespace CompillerServices.Backend.Writers
{
    public class CppFileWriterFactory : ISourceWriterFactory
    {
        private const string CppSourceExtension = ".cpp";
        private const string CppHeaderExtension = ".h";

        public ISourceWriter Create(FileInfo moduleFile, DirectoryInfo outputDirectory)
        {
            if (!outputDirectory.Exists)
            {
                outputDirectory.Create();
            }

            string nameWithoutExtension = moduleFile.GetShortNameWithoutExtension();

            string headerFullName =
                Path.Combine(outputDirectory.FullName, $"{nameWithoutExtension}{CppHeaderExtension}");
            string sourceFullName =
                Path.Combine(outputDirectory.FullName, $"{nameWithoutExtension}{CppSourceExtension}");

            FileStream headerFileStream = File.Open(headerFullName, FileMode.Create, FileAccess.Write);
            FileStream sourceFileStream = File.Open(sourceFullName, FileMode.Create, FileAccess.Write);

            return new CppWriter(moduleFile.GetShortNameWithoutExtension() + CppHeaderExtension, new StreamWriter(headerFileStream), new StreamWriter(sourceFileStream));
        }
    }
}