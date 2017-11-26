using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.Exit;
using LanguageServerProtocol.IPC.Window;

namespace PluginServer.Handlers
{
    public class ExitHandler : DefaultExitHandler
    {
        private readonly IWindowMessageSender _messageSender;

        public ExitHandler(IWindowMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public override async Task Handle()
        {
            await _messageSender.LogMessage(MessageType.Info, "Exit");
        }
    }
}