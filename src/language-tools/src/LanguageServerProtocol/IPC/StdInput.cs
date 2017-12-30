using System;

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