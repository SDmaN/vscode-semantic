using System;
using System.IO;
using System.Threading.Tasks;
using CompillerServices.Backend;
using CompillerServices.Backend.Writers;
using CompillerServices.DependencyInjection;
using JsonRpc;
using JsonRpc.DependencyInjection;
using LanguageServerProtocol;
using LanguageServerProtocol.IPC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PluginServer.Settings;
using SlangGrammar.Factories;

namespace PluginServer
{
    internal static class Program
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

            serviceCollection.Configure<LanguageOptions>(configuration.GetSection("LanguageOptions"));

            serviceCollection.AddStdIo();
            serviceCollection.AddJsonRpc();
            serviceCollection.AddLsp();

            serviceCollection.AddSingleton<ILexerFactory, LexerFactory>();
            serviceCollection.AddSingleton<IParserFactory, ParserFactory>();

            serviceCollection.AddCompillers();

            IServiceProvider provider = serviceCollection.BuildServiceProvider();

            var b = provider.GetService<IBackendCompiller>();
            await b.Compile(new DirectoryInfo("C:/Users/sdman/Desktop/semlang/"),
                new DirectoryInfo("C:/Users/sdman/Desktop/semlang/out/"),
                (p, r) => Path.GetRelativePath(p.FullName, r.FullName));

            /*StringWriter h = new StringWriter();
            StringWriter s = new StringWriter();

            CppTextWriterFactory wf = new CppTextWriterFactory(h, s);
            BackendCompiller compiller = new BackendCompiller(provider.GetService<ILexerFactory>(),
                provider.GetService<IParserFactory>(), wf);

            await compiller.Compile(new FileInfo("C:/Users/sdman/Desktop/semlang/main.slang"), "C:/Users/sdman/Desktop/semlang/out");

            Console.WriteLine("header:");
            Console.WriteLine(h.ToString());

            Console.WriteLine();
            Console.WriteLine("source:");
            Console.WriteLine(s.ToString());*/

            return;

            IRpcService service = provider.GetService<IRpcService>();

            while (true)
            {
                await service.HandleMessage();
            }
        }
    }
}