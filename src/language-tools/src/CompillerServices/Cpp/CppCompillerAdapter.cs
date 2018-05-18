using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CompillerServices.IO;
using CompillerServices.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CompillerServices.Cpp
{
    public class GccAdapter : ICppCompillerAdapter
    {
        private readonly IOptions<CppCompillerOptions> _compillerOptions;
        private readonly IStringLocalizer<GccAdapter> _localizer;
        private readonly ILogger<GccAdapter> _logger;

        public GccAdapter(ILogger<GccAdapter> logger, IStringLocalizer<GccAdapter> localizer,
            IOptions<CppCompillerOptions> compillerOptions)
        {
            _logger = logger;
            _localizer = localizer;
            _compillerOptions = compillerOptions;
        }

        public async Task Build(SourceContainer sources, DirectoryInfo outputDirectory)
        {
            var gccDirectory = new DirectoryInfo(_compillerOptions.Value.CppCompillerPath);

            if (!gccDirectory.Exists)
            {
                throw new DirectoryNotFoundException(_localizer["Compiler directory {0} not exists.",
                    gccDirectory.FullName]);
            }

            DirectoryInfo binDirectory = outputDirectory.CreateSubdirectory(Constants.CppOutput);

            _logger.LogCompillerBuilds(outputDirectory.FullName);

            string gccFileName = Path.Combine(gccDirectory.FullName, Constants.CppCompillerName);

            outputDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly);

            string gccArgs = await GetGccArgs(sources, outputDirectory);

            var processStartInfo = new ProcessStartInfo(gccFileName, gccArgs)
            {
                WorkingDirectory = gccDirectory.FullName
            };

            using (Process gppProcess = Process.Start(processStartInfo))
            {
                gppProcess.WaitForExit();

                if (gppProcess.ExitCode == 0)
                {
                    await PostBuild(gccDirectory, binDirectory);
                }
            }
        }

        private static Task<string> GetGccArgs(SourceContainer sources, DirectoryInfo outputDirectory)
        {
            var builder = new StringBuilder();
            builder.Append(
                $"-o {Path.Combine(outputDirectory.FullName, Constants.CppOutput, sources.ProjectFile.GetShortNameWithoutExtension())}");
            builder.Append($" {Path.Combine(outputDirectory.FullName, "*" + Constants.CppSourceExtension)}");

            foreach (DirectoryInfo subDir in outputDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (subDir.Name == Constants.CppOutput)
                {
                    continue;
                }

                builder.Append($" {Path.Combine(subDir.FullName, "*" + Constants.CppSourceExtension)}");
            }

            return Task.FromResult(builder.ToString());
        }

        private static Task PostBuild(DirectoryInfo gccDirectory, DirectoryInfo binDirectory)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IEnumerable<FileInfo> neededLibraries = gccDirectory.EnumerateFiles()
                    .Where(x => Constants.WindowsNeededLibraries.Contains(x.Name));

                foreach (FileInfo neededLibrary in neededLibraries)
                {
                    neededLibrary.CopyTo(Path.Combine(binDirectory.FullName, neededLibrary.Name));
                }
            }

            return Task.CompletedTask;
        }
    }
}