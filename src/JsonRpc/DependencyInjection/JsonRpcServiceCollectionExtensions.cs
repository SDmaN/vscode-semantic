using System;
using System.Collections.Generic;
using JsonRpc.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace JsonRpc.DependencyInjection
{
    public static class JsonRpcServiceCollectionExtensions
    {
        public static void AddJsonRpc(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            IDictionary<string, Type> handlers = HandlerHelper.CollectHandlerTypes();

            foreach (Type handlerType in handlers.Values)
            {
                serviceCollection.AddTransient(handlerType);
            }

            serviceCollection.AddSingleton<IHandlerFactory, ServiceProviderHandlerFactory>();
            serviceCollection.AddScoped<IRpcService, RpcService>();
        }
    }
}