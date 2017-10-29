using System;
using System.Collections.Generic;
using JsonRpc.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JsonRpc.DependencyInjection
{
    public static class JsonRpcServiceCollectionExtensions
    {
        public static void AddRpcHandlers(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            IDictionary<string, Type> handlers = HandlerHelper.CollectHandlerTypes();

            foreach (Type handlerType in handlers.Values)
            {
                serviceCollection.TryAddTransient(handlerType);
            }
        }

        public static void AddJsonRpc(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            AddRpcHandlers(serviceCollection);

            serviceCollection.AddSingleton<IHandlerFactory, ServiceProviderHandlerFactory>();

            serviceCollection.AddScoped<IRequestCancellationManager, RequestCancellationManager>();
            serviceCollection.AddScoped<IRpcService, RpcService>();
        }
    }
}