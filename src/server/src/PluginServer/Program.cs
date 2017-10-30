using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.DependencyInjection;
using LanguageServerProtocol;
using LanguageServerProtocol.IPC;
using Microsoft.Extensions.DependencyInjection;

namespace PluginServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG && WAIT_FOR_DEBUGGER
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(1000);
            }

            Debugger.Break();
#endif
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddJsonRpc();

            IServiceProvider provider = serviceCollection.BuildServiceProvider();

            Task.Run(async () =>
            {
                IRpcService service = provider.GetService<IRpcService>();

                using (StreamInput input = new StreamInput(Console.OpenStandardInput()))
                using (StreamOutput output = new StreamOutput(Console.OpenStandardOutput()))
                {
                    while (true)
                    {
                        await service.HandleRequest(input, output);
                    }
                }
            }).Wait();
        }

        public class InitializeHandler : BaseInitializeHandler
        {
            public override async Task Handle(long processId, string rootPath, string rootUri, object capabilities,
                string trace)
            {
                using (FileStream fs = new FileStream("C:/users/sdman/Desktop/1.txt", FileMode.Create))
                {
                    StreamWriter writer = new StreamWriter(fs);
                    writer.AutoFlush = true;
                    await writer.WriteLineAsync(processId.ToString());
                }
            }
        }
    }
}