using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace CompillerServices
{
    internal static class Constants
    {
        public const string CppSourceExtension = ".cpp";
        public const string CppHeaderExtension = ".h";

        public const string SlangExtension = ".slang";
        public const string SlangFileMask = "*" + SlangExtension;

        public const string CppCompillerName = "g++";

        public const string CppOutput = "bin";
        public static readonly string CppCompillerPath;
        public static readonly string SystemModulesPath;

        public static readonly IEnumerable<string> WindowsNeededLibraries = new List<string>
        {
            "libgcc_s_dw2-1.dll",
            "libstdc++-6.dll",
            "libwinpthread-1.dll"
        };

        static Constants()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#if DEBUG
                CppCompillerPath = "mingw/mingw32/bin";
#else
                CppCompillerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mingw/mingw32/bin");
#endif
                SystemModulesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemModules");
            }


        }

        public static class TypeKeywords
        {
            public const string Bool = "bool";
            public const string Int = "int";
            public const string Real = "real";
        }

        public static class ArgModifiers
        {
            public const string Val = "val";
            public const string Ref = "ref";
        }

        public static class AccessModifiers
        {
            public const string Private = "private";
            public const string Public = "public";
        }
    }
}