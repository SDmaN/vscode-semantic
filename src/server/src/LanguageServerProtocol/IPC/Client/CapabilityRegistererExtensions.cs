using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc.Messages;

namespace LanguageServerProtocol.IPC.Client
{
    public static class CapabilityRegistererExtensions
    {
        public static Task<IResponse> RegisterCapability(this ICapabilityRegisterer capabilityRegisterer,
            IEnumerable<Registration> registrations)
        {
            if (capabilityRegisterer == null)
            {
                throw new ArgumentNullException(nameof(capabilityRegisterer));
            }

            return capabilityRegisterer.RegisterCapability(new RegistrationParams
            {
                Registrations = registrations
            });
        }
    }
}