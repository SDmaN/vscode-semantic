using System.IO;
using Microsoft.Extensions.Localization;

namespace CompillerServices.Backend.Translators
{
    internal class CppTranslatorFactory : ITranslatorFactory
    {
        private readonly IStringLocalizer<CppVisitorTranslator> _translatorLocalizer;

        public CppTranslatorFactory(IStringLocalizer<CppVisitorTranslator> translatorLocalizer)
        {
            _translatorLocalizer = translatorLocalizer;
        }

        public ITranslator Create(FileInfo moduleFile, DirectoryInfo outputDirectory)
        {
            if (!outputDirectory.Exists)
            {
                outputDirectory.Create();
            }

            string nameWithoutExtension = moduleFile.GetShortNameWithoutExtension();

            string headerFullName =
                Path.Combine(outputDirectory.FullName, $"{nameWithoutExtension}{Constants.CppHeaderExtension}");
            string sourceFullName =
                Path.Combine(outputDirectory.FullName, $"{nameWithoutExtension}{Constants.CppSourceExtension}");

            FileStream headerFileStream = File.Open(headerFullName, FileMode.Create, FileAccess.Write);
            FileStream sourceFileStream = File.Open(sourceFullName, FileMode.Create, FileAccess.Write);

            return new CppVisitorTranslator(_translatorLocalizer,
                moduleFile.GetShortNameWithoutExtension() + Constants.CppHeaderExtension, new StreamWriter(headerFileStream),
                new StreamWriter(sourceFileStream));
        }
    }
}