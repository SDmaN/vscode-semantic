using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.Handlers.TextDocument.Completion;

namespace PluginServer.Handlers
{
    public class CompletionHandler : DefaultCompletionHandler
    {
        public override Task<IRpcHandleResult<IEnumerable<CompletionItem>>> Handle(TextDocumentIdentifier textDocument,
            Position position, CompletionContext context)
        {
            IEnumerable<CompletionItem> result = new List<CompletionItem>
            {
                new CompletionItem
                {
                    Label = "Completion method",
                    Kind = CompletionItemKind.Method,
                    Detail = "HUMAN DETAIL",
                    Documentation = new MarkupContent
                    {
                        Kind = MarkupKind.Markdown,
                        Value = "##DOCS"
                    },
                    TextEdit = new TextEdit
                    {
                        NewText = "Hi there!",
                        Range = new Range
                        {
                            Start = position,
                            End = new Position(position.Line, position.Character + "Hi there!".Length)
                        }
                    }
                },
                new CompletionItem
                {
                    Label = "Completion value",
                    Kind = CompletionItemKind.Value,
                    Detail = "HUMAN DETAIL",
                    Documentation = new MarkupContent
                    {
                        Kind = MarkupKind.Markdown,
                        Value = "##DOCS"
                    }
                }
            };

            return Task.FromResult(Ok(result));
        }
    }
}