using System.IO;

namespace CompillerServices.Backend.Writers
{
    public class CppTextWriterFactory : ISourceWriterFactory
    {
        private readonly TextWriter _headerWriter;
        private readonly TextWriter _sourceWriter;

        public CppTextWriterFactory(TextWriter headerWriter, TextWriter sourceWriter)
        {
            _headerWriter = headerWriter;
            _sourceWriter = sourceWriter;
        }

        public ISourceWriter Create(FileInfo moduleFile, DirectoryInfo outputDirectory)
        {
            return new CppWriter(_headerWriter, _sourceWriter);
        }
    }

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

            string nameWithoutExtension = Path.GetFileNameWithoutExtension(moduleFile.Name);

            string headerFullName =
                Path.Combine(outputDirectory.FullName, $"{nameWithoutExtension}{CppHeaderExtension}");
            string sourceFullName =
                Path.Combine(outputDirectory.FullName, $"{nameWithoutExtension}{CppSourceExtension}");

            FileStream headerFileStream = File.Open(headerFullName, FileMode.Create, FileAccess.Write);
            FileStream sourceFileStream = File.Open(sourceFullName, FileMode.Create, FileAccess.Write);

            return new CppWriter(new StreamWriter(headerFileStream), new StreamWriter(sourceFileStream));
        }
    }
}