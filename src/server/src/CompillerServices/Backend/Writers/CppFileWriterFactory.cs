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

        public ISourceWriter Create(string inputModuleFileName = null, string outputPath = null)
        {
            return new CppWriter(_headerWriter, _sourceWriter);
        }
    }

    public class CppFileWriterFactory : ISourceWriterFactory
    {
        private const string CppSourceExtension = ".cpp";
        private const string CppHeaderExtension = ".h";

        public ISourceWriter Create(string inputModuleFileName, string outputPath)
        {
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(inputModuleFileName);

            string headerFullName = Path.Combine(outputPath, $"{nameWithoutExtension}{CppHeaderExtension}");
            string sourceFullName = Path.Combine(outputPath, $"{nameWithoutExtension}{CppSourceExtension}");

            FileStream headerFileStream = File.Open(headerFullName, FileMode.Create, FileAccess.Write);
            FileStream sourceFileStream = File.Open(sourceFullName, FileMode.Create, FileAccess.Write);

            return new CppWriter(new StreamWriter(headerFileStream), new StreamWriter(sourceFileStream));
        }
    }
}