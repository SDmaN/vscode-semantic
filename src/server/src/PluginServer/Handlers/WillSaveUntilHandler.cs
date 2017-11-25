using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.Handlers.TextDocument.WillSave;

namespace PluginServer.Handlers
{
    public class WillSaveUntilHandler : DefaultWillSaveWaitUntilHandler
    {
        public override Task<IRpcHandleResult<IList<TextEdit>>> Handle(TextDocumentIdentifier textDocument,
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
                    NewText = "ХУУУЙ"
                }
            };

            return Task.FromResult(Ok(edits));
        }
    }
}