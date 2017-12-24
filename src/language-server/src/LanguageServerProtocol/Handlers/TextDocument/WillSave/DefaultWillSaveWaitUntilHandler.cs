using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc.Handlers;
using JsonRpc.HandleResult;

namespace LanguageServerProtocol.Handlers.TextDocument.WillSave
{
    [RemoteMethodHandler("textDocument/willSaveWaitUntil")]
    public abstract class DefaultWillSaveWaitUntilHandler : RemoteMethodHandler
    {
        public abstract Task<IRpcHandleResult<IList<TextEdit>>> Handle(TextDocumentIdentifier textDocument,
            TextDocumentSaveReason reason);
    }
}