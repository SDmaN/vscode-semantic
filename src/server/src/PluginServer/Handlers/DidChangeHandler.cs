using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.TextDocument;

namespace PluginServer.Handlers
{
    public class DidChangeHandler : DefaultDidChangeHandler
    {
        public override Task Handle(VersionedTextDocumentIdentifier textDocument,
            IList<TextDocumentContentChangeEvent> contentChanges)
        {
            return Task.CompletedTask;
        }
    }
}