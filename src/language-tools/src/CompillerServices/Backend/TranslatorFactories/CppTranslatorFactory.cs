using System.IO;
using CompillerServices.Backend.Translators;

namespace CompillerServices.Backend.TranslatorFactories
{
    public class CppTranslatorFactory : ITranslatorFactory
    {
        private const string CppSourceExtension = ".cpp";
        private const string CppHeaderExtension = ".h";

        public ITranslator Create(FileInfo moduleFile, DirectoryInfo outputDirectory)
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

            return new CppVisitorTranslator(moduleFile.GetShortNameWithoutExtension() + CppHeaderExtension,
                new StreamWriter(headerFileStream), new StreamWriter(sourceFileStream));
        }
    }
}