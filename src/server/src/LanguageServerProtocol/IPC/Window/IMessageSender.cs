using System.Threading.Tasks;

namespace LanguageServerProtocol.IPC.Window
{
    public interface IMessageSender
    {
        Task ShowMessage(ShowMessageParams showMessageParams);
    }
}