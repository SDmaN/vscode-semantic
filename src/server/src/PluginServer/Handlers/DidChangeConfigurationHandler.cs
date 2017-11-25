using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.Workspace.DidChangeConfiguration;

namespace PluginServer.Handlers
{
    public class DidChangeConfigurationHandler : DefaultDidChangeConfigurationHandler
    {
        public override Task Handle(object settings)
        {
            return Task.CompletedTask;
        }
    }
}