using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Initialize;

namespace PluginServer.Handlers
{
    public class InitializeHandler : DefaultInitializeHandler
    {
        public override Task<IRpcHandleResult<InitializeResult>> Handle(long processId, string rootPath,
            Uri rootUri, ClientCapabilities capabilities, string trace)
        {
            InitializeResult initResult = new InitializeResult
            {
                Capabilities = new ServerCapabilities
                {
                    HoverProvider = true,
                    RenameProvider = true
                }
            };

            IRpcHandleResult<InitializeResult> handleResult = new SuccessResult<InitializeResult>(initResult);
            return Task.FromResult(handleResult);
        }
    }
}