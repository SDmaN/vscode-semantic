using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Atn;
using JsonRpc;
using JsonRpc.DependencyInjection;
using LanguageServerProtocol;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.IPC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PluginServer.LanguageServices;
using PluginServer.Settings;

namespace PluginServer
{
    internal class ParserStack
    {
        private readonly IEnumerable<ATNState> _states;

        public ParserStack(IEnumerable<ATNState> states)
        {
            _states = states;
        }

        public (bool IsSuccess, ParserStack ParserStack) Process(ATNState state)
        {
            switch (state)
            {
                case RuleStartState _:
                case StarBlockStartState _:
                case BasicBlockStartState _:
                case PlusBlockStartState _:
                case StarLoopEntryState _:
                    return (true, new ParserStack(_states.Append(state)));

                case BlockEndState blockEndState:
                    return _states.Last().Equals(blockEndState.startState)
                        ? (true, new ParserStack(_states.SkipLast(1)))
                        : (false, this);

                case LoopEndState loopEndState:
                {
                    bool cont = _states.Last() is StarLoopEntryState last &&
                                last.loopBackState.Equals(loopEndState.loopBackState);

                    return cont ? (true, new ParserStack(_states.SkipLast(1))) : (false, this);
                }

                case RuleStopState ruleStopState:
                    bool cont1 = _states.Last() is RuleStartState last1 && last1.stopState.Equals(ruleStopState);
                    return cont1 ? (true, new ParserStack(_states.SkipLast(1))) : (false, this);

                case BasicState _:
                case StarLoopbackState _:
                case PlusLoopbackState _:
                    return (true, this);

                default:
                    throw new InvalidOperationException($"{nameof(ParserStack)} exception.");
            }
        }
    }

    internal static class ParserStackExtensions
    {
        public static bool IsCompatible(this ParserStack parserStack, ATNState state)
        {
            if (parserStack == null)
            {
                throw new ArgumentNullException(nameof(parserStack));
            }

            (bool IsSuccess, ParserStack ParserStack) result = parserStack.Process(state);

            return !state.OnlyHasEpsilonTransitions ||
                   state.Transitions.Any(x => result.ParserStack.IsCompatible(x.target));
        }
    }

    internal class EditorContext
    {
        public EditorContext(string code)
        {
        }
    }

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
            serviceCollection.AddSingleton<ICompletionService, CompletionService>();
            serviceCollection.AddSingleton<IFileContentKeeper, FileContentKeeper>();

            IServiceProvider provider = serviceCollection.BuildServiceProvider();

            IFileContentKeeper contentKeeper = provider.GetService<IFileContentKeeper>();
            Uri uri = new Uri("http://google.com");
            contentKeeper.Add(uri,
                @"   
input 
");

            ICompletionService completionService = provider.GetService<ICompletionService>();
            completionService.GetCompletionItems(new TextDocumentIdentifier
                {
                    Uri = uri
                },
                new Position(0, 3));

            return;

            IRpcService service = provider.GetService<IRpcService>();

            while (true)
            {
                await service.HandleMessage();
            }
        }
    }
}