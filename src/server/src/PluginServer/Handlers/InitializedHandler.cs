using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.Initialized;

namespace PluginServer.Handlers
{
    public class InitializedHandler : DefaultInitilalizedHandler
    {
        public override Task Handle()
        {
            return Task.CompletedTask;
        }
    }
}