using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.HandleResult;
using JsonRpc.Messages;
using LanguageServerProtocol.IPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestingApp
{
    internal class A
    {
        public string Name { get; set; }
    }

    internal class Input : IInput
    {
        public Task<JToken> ReadAsync(CancellationToken cancellationToken = default)
        {
            Request request = new Request(new MessageId(123), "abc", new object[] { "add" });

            return Task.FromResult(JToken.FromObject(
                request
            ));
        }
    }

    [RemoteMethodHandler("abc")]
    internal class Handler
    {
        public static Task<ErrorResult> Handle(string s)
        {
            Console.WriteLine($"Handled: ");

            throw new Exception("EXCEPTION BLYAT");

            return Task.FromResult(
                new ErrorResult(new Error(ErrorCode.InternalError, "abcdefg", new { ABC = 5000 }))
            );
        }
    }

    internal class Output : IOutput
    {
        public Task WriteAsync(JToken response, CancellationToken cancellationToken = default)
        {
            Console.WriteLine(response);
            return Task.CompletedTask;
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                string json = JsonConvert.SerializeObject(new A { Name = "123" });
                Console.WriteLine(json);
                

                StreamInput streamInput = new StreamInput(Console.OpenStandardInput());
                
                RpcService service = new RpcService();
                service.RegisterHandler(new Handler());

                await service.HandleRequest(streamInput, new Output());
            }).Wait();
        }

        private static Task M(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                }
            }, cancellationToken);
        }
    }
}