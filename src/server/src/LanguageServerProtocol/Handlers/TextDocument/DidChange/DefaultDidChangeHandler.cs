using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc.Handlers;

namespace LanguageServerProtocol.Handlers.TextDocument.DidChange
{
    [RemoteMethodHandler("textDocument/didChange")]
    public abstract class DefaultDidChangeHandler : RemoteMethodHandler
    {
        public abstract Task Handle(VersionedTextDocumentIdentifier textDocument,
            IList<TextDocumentContentChangeEvent> contentChanges);
    }
}