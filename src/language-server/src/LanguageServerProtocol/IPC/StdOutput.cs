using System;

namespace LanguageServerProtocol.IPC
{
    public sealed class StdOutput : StreamOutput
    {
        public StdOutput()
            : base(Console.OpenStandardOutput())
        {
        }
    }
}