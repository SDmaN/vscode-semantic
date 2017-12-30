using System;
using JsonRpc;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageServerProtocol.IPC
{
    public static class StdIoServiceCollectionExtensions
    {
        public static void AddStdIo(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddScoped<IInput, StdInput>();
            serviceCollection.AddScoped<IOutput, StdOutput>();
        }
    }
}