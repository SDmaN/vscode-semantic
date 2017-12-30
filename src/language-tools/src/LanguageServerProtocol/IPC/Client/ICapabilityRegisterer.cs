using System.Threading.Tasks;
using JsonRpc.Messages;

namespace LanguageServerProtocol.IPC.Client
{
    public interface ICapabilityRegisterer
    {
        Task<IResponse> RegisterCapability(RegistrationParams registrations);
    }
}