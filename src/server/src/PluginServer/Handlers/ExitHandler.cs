using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.Exit;

namespace PluginServer.Handlers
{
    public class ExitHandler : DefaultExitHandler
    {
        public override Task Handle()
        {
            return Task.CompletedTask;
        }
    }
}