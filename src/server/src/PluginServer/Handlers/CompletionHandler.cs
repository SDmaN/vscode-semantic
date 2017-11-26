using System.Threading.Tasks;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.Handlers.TextDocument.Completion;

namespace PluginServer.Handlers
{
    public class CompletionHandler : DefaultCompletionHandler
    {
        public override Task<IRpcHandleResult<CompletionList>> Handle(TextDocumentIdentifier textDocument,
            Position position, CompletionContext context)
        {
            CompletionList result = new CompletionList
            {
                IsIncomplete = false,
                Items = new[]
                {    
                    new CompletionItem
                    {
                        Label = "Completion 1",
                        Kind = CompletionItemKind.Value,
                        Detail = "HUMAN DETAIL",
                        Documentation = new MarkupContent
                        {
                            Kind = MarkupKind.PlainText,
                            Value = "DOCS"
                        },
                        
                    }
                }
            };

            return Task.FromResult(Ok(result));
        }
    }
}