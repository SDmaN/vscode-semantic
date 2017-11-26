using System.Threading.Tasks;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Handlers.Shutdown;
using LanguageServerProtocol.IPC.Window;

namespace PluginServer.Handlers
{
    public class ShutdownHandler : DefaultShutdownHandler
    {
        private readonly IWindowMessageSender _messageSender;

        public ShutdownHandler(IWindowMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public override async Task<IRpcHandleResult> Handle()
        {
            await _messageSender.LogMessage(MessageType.Info, "Shutdown");
            return Ok(null);
        }
    }
}