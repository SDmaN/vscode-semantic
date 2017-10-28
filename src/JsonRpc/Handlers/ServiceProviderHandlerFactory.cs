using System;
using System.Collections.Generic;
using JsonRpc.Exceptions;

namespace JsonRpc.Handlers
{
    public class ServiceProviderHandlerFactory : IHandlerFactory
    {
        private readonly IDictionary<string, Type> _handlers;
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _handlers = HandlerHelper.CollectHandlerTypes();
        }

        public RemoteMethodHandler CreateHandler(string method)
        {
            if (!_handlers.TryGetValue(method.ToLower(), out Type handlerType))
            {
                throw new HandlerNotFoundException(method, $"Handler for {method} method not found.");
            }

            RemoteMethodHandler handler = (RemoteMethodHandler) _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                throw new HandlerNotFoundException(method, $"Handler for {method} method not found.");
            }

            return handler;
        }
    }
}