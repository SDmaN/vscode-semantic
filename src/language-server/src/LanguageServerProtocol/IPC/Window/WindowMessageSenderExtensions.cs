using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageServerProtocol.IPC.Window
{
    public static class WindowMessageSenderExtensions
    {
        public static Task ShowMessage(this IWindowMessageSender messageSender, MessageType type, string message,
            CancellationToken cancellationToken = default)
        {
            return messageSender.ShowMessage(new ShowMessageParams
            {
                Type = type,
                Message = message
            }, cancellationToken);
        }

        public static Task<MessageActionItem> ShowMessageRequest(this IWindowMessageSender messageSender,
            MessageType type, string message, IList<MessageActionItem> actions,
            CancellationToken cancellationToken = default)
        {
            return messageSender.ShowMessageRequest(new ShowMessageRequestParams
            {
                Type = type,
                Message = message,
                Actions = actions
            }, cancellationToken);
        }

        public static Task LogMessage(this IWindowMessageSender messageSender, MessageType type, string message,
            CancellationToken cancellationToken = default)
        {
            return messageSender.LogMessage(new LogMessageParams
            {
                Type = type,
                Message = message
            }, cancellationToken);
        }
    }
}