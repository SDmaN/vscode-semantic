using System;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.DependencyInjection;
using JsonRpc.Handlers;
using JsonRpc.HandleResult;
using JsonRpc.Messages;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace TestingApp
{
    internal class A
    {
        public string Name { get; set; }
    }

    internal class Input : IInput
    {
        private int _count;

        public async Task<JToken> ReadAsync(CancellationToken cancellationToken = default)
        {
            Request request;

            if (_count == 0)
            {
                request = new Request(new MessageId(1), "abc", null);

                _count++;
            }

            else if (_count == 1)
            {
                request = new Request(new MessageId(null), "$/cancelRequest", new { id = 1L });
                _count++;
            }
            else
            {
                request = null;

                await Task.Delay(5000);
            }

            return JToken.FromObject(
                request
            );
        }
    }

    [RemoteMethodHandler("abc")]
    internal class Handler : RemoteMethodHandler
    {
        public Task<ErrorResult> Handle()
        {
            while (true)
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            return Task.FromResult(
                new ErrorResult(new Error(ErrorCode.InternalError, "abcdefg", new { ABC = 5000 }))
            );
        }
    }

    internal class Output : IOutput
    {
        private readonly object _locker = new object();

        public Task WriteAsync(JToken response, CancellationToken cancellationToken = default)
        {
            lock (_locker)
            {
                Console.WriteLine(response);
            }

            return Task.CompletedTask;
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            IServiceCollection col = new ServiceCollection();
            col.AddJsonRpc();

            IRpcService rpcService = col.BuildServiceProvider().GetRequiredService<IRpcService>();

            // Content-Length: 44
            // {"jsonrpc": "2.0", "id": 1, "method": "abc"}

            // Content-Length: 62
            // {"jsonrpc": "2.0", "method": "$/cancelRequest", "params": [1]}

            Input input = new Input();
            Output output = new Output();

            while (true)
            {
                await rpcService.HandleRequest(input, output);
            }
        }
    }
}