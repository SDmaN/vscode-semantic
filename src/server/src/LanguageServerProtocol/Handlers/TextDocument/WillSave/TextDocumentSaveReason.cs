namespace LanguageServerProtocol.Handlers.TextDocument.WillSave
{
    public enum TextDocumentSaveReason
    {
        Manual = 1,
        AfterDelay = 2,
        FocusOut = 3
    }
}