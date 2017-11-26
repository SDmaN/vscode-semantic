using System;
using LanguageServerProtocol.IPC.Client;
using LanguageServerProtocol.IPC.Window;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageServerProtocol
{
    public static class LanguageServerProtocolServiceCollectionExtensions
    {
        public static void AddLsp(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddTransient<IWindowMessageSender, WindowMessageSender>();
            serviceCollection.AddTransient<ICapabilityRegisterer, CapabilityRegisterer>();
        }
    }
}