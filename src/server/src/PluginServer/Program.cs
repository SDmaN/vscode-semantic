using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.DependencyInjection;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Initialize;
using LanguageServerProtocol.IPC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PluginServer
{
    internal class Program
    {
        internal static async Task Main(string[] args)
        {
#if DEBUG && WAIT_FOR_DEBUGGER
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(1000);
            }

            Debugger.Break();
#endif
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddDebug();
            });

            serviceCollection.AddJsonRpc();

            IServiceProvider provider = serviceCollection.BuildServiceProvider();

            IRpcService service = provider.GetService<IRpcService>();

            using (StreamInput input = new StreamInput(Console.OpenStandardInput()))
            using (StreamOutput output = new StreamOutput(Console.OpenStandardOutput()))
            {
                while (true)
                {
                    await service.HandleRequest(input, output);
                }
            }
        }

        public class InitializeHandler : DefaultInitializeHandler
        {
            public override async Task<IRpcHandleResult<InitializeResult>> Handle(long processId, string rootPath,
                Uri rootUri, ClientCapabilities capabilities, string trace)
            {
                return new SuccessResult<InitializeResult>(new InitializeResult());
            }
        }
    }
}