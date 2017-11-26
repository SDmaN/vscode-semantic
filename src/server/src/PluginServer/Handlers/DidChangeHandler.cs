using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.Handlers.TextDocument.DidChange;
using LanguageServerProtocol.IPC.Window;

namespace PluginServer.Handlers
{
    public class DidChangeHandler : DefaultDidChangeHandler
    {
        private readonly IWindowMessageSender _messageSender;

        public DidChangeHandler(IWindowMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public override async Task Handle(VersionedTextDocumentIdentifier textDocument,
            IList<TextDocumentContentChangeEvent> contentChanges)
        {
            await _messageSender.LogMessage(MessageType.Info, "Did change");
        }
    }
}