using System.Collections.Generic;

namespace LanguageServerProtocol.Handlers.TextDocument.Completion
{
    public class CompletionList
    {
        public bool IsIncomplete { get; set; }
        public IEnumerable<CompletionItem> Items { get; set; }
    }
}