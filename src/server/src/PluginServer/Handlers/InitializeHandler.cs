using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.HandleResult;
using JsonRpc.Messages;
using LanguageServerProtocol.Handlers.Initialize;
using Newtonsoft.Json.Linq;

namespace PluginServer.Handlers
{
    public class InitializeHandler : DefaultInitializeHandler
    {
        private readonly IOutput _output;

        public InitializeHandler(IOutput output)
        {
            _output = output;
        }
        
        public override async Task<IRpcHandleResult<InitializeResult>> Handle(long processId, string rootPath,
            Uri rootUri, ClientCapabilities capabilities, string trace)
        {
            IRequest r = new Request(MessageId.Empty, "window/showMessage", new { type = 2, message = "abcdefg!" });
            JToken jt = JToken.FromObject(r);
            await _output.WriteAsync(jt);
            
            InitializeResult initResult = new InitializeResult
            {
                Capabilities = new ServerCapabilities
                {
                    HoverProvider = true,
                    RenameProvider = true,
                    ExecuteCommandProvider = new ExecuteCommandOptions
                    {
                        Commands = new List<string>
                        {
                            "Command 1",
                            "Command 2"
                        }
                    }
                }
            };

            IRpcHandleResult<InitializeResult> handleResult = new SuccessResult<InitializeResult>(initResult);
            return handleResult;
        }
    }
}