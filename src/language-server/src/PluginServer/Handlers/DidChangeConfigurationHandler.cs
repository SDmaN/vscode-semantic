using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.Workspace.DidChangeConfiguration;
using LanguageServerProtocol.IPC.Window;

namespace PluginServer.Handlers
{
    public class DidChangeConfigurationHandler : DefaultDidChangeConfigurationHandler
    {
        private readonly IWindowMessageSender _windowMessageSender;

        public DidChangeConfigurationHandler(IWindowMessageSender windowMessageSender)
        {
            _windowMessageSender = windowMessageSender;
        }

        public override async Task Handle(object settings)
        {
            await _windowMessageSender.LogMessage(MessageType.Info, "Did change configuration");
        }
    }
}