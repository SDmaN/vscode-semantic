using System.IO;

namespace CompillerServices
{
    internal static class FileInfoExtensions
    {
        public static string GetShortNameWithoutExtension(this FileInfo fileInfo)
        {
            return Path.GetFileNameWithoutExtension(fileInfo.Name);
        }
    }
}