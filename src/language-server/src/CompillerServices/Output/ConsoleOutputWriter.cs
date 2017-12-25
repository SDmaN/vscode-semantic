using System;

namespace CompillerServices.Output
{
    public class ConsoleOutputWriter : StreamOutputWriter
    {
        public ConsoleOutputWriter()
            : base(Console.OpenStandardOutput())
        {
        }
    }
}