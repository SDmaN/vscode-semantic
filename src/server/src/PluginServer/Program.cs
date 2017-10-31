using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.DependencyInjection;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Initialize;
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
            public override Task<IRpcHandleResult<InitializeResult>> Handle(long processId, string rootPath, Uri rootUri, ClientCapabilities capabilities, string trace)
            {
                
            }
        }
    }
}