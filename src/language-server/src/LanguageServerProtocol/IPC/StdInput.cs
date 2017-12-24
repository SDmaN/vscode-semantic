using System;
using System.IO;

namespace LanguageServerProtocol.IPC
{
    public sealed class StdInput : StreamInput
    {
        public StdInput()
            : base(Console.OpenStandardInput())
        {
        }
    }
}