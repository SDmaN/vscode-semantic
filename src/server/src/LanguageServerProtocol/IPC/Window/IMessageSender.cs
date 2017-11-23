using System.Threading;
using System.Threading.Tasks;

namespace LanguageServerProtocol.IPC.Window
{
    public interface IMessageSender
    {
        Task ShowMessage(ShowMessageParams showMessageParams, CancellationToken cancellationToken = default);

        Task<MessageActionItem> ShowMessageRequest(ShowMessageRequestParams showMessageRequestParams,
            CancellationToken cancellationToken = default);
    }
}