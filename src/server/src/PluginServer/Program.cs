using System;
using System.Diagnostics;
using System.Threading;

namespace PluginServer
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG && WAIT_FOR_DEBUGGER
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(1000);
            }

            Debugger.Break();
#endif
            Console.WriteLine("Hello World!");
        }
    }
}
