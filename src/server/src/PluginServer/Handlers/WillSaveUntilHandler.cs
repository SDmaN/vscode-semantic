using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.Handlers.TextDocument.WillSave;
using LanguageServerProtocol.IPC.Window;

namespace PluginServer.Handlers
{
    public class WillSaveUntilHandler : DefaultWillSaveWaitUntilHandler
    {
        private readonly IWindowMessageSender _messageSender;

        public WillSaveUntilHandler(IWindowMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public override async Task<IRpcHandleResult<IList<TextEdit>>> Handle(TextDocumentIdentifier textDocument,
            TextDocumentSaveReason reason)
        {
            IList<TextEdit> edits = new List<TextEdit>
            {
                new TextEdit
                {
                    Range = new Range
                    {
                        Start = new Position(0, 0),
                        End = new Position(0, 1)
                    },
                    NewText = "Saved"
                }
            };

            await _messageSender.LogMessage(MessageType.Info, "Will save until");
            return Ok(edits);
        }
    }
}