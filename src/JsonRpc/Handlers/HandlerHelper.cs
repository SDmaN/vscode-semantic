using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonRpc.Exceptions;

namespace JsonRpc.Handlers
{
    internal static class HandlerHelper
    {
        internal static IDictionary<string, Type> CollectHandlerTypes()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            IEnumerable<KeyValuePair<string, Type>> entryHandlers = GetAssemblyHandlers(entryAssembly);

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            IEnumerable<KeyValuePair<string, Type>> executingHandlers = GetAssemblyHandlers(executingAssembly);

            return new ConcurrentDictionary<string, Type>(entryHandlers.Concat(executingHandlers));
        }

        private static IEnumerable<KeyValuePair<string, Type>> GetAssemblyHandlers(Assembly assembly)
        {
            IList<KeyValuePair<string, Type>> collectedHandlers = new List<KeyValuePair<string, Type>>();

            IEnumerable<Type> handlerTypes =
                assembly.GetTypes().Where(x =>
                    x.IsDefined(typeof(RemoteMethodHandlerAttribute)) &&
                    typeof(RemoteMethodHandler).IsAssignableFrom(x));

            foreach (Type handlerType in handlerTypes)
            {
                RemoteMethodHandlerAttribute handlerAttribute =
                    handlerType.GetCustomAttribute<RemoteMethodHandlerAttribute>();

                if (string.IsNullOrWhiteSpace(handlerAttribute.MethodName))
                {
                    throw new HandleMethodNotSpecifiedException(
                        $"Handle method is not specified for type {handlerType.FullName}");
                }

                MethodInfo handleMethod = GetHandleMethod(handlerType);

                if (handleMethod == null)
                {
                    throw new HandleMethodNotFoundException(handlerAttribute.MethodName, handlerType,
                        $"Method {Constants.HandleMethodName} not found in {handlerType.FullName}.");
                }

                collectedHandlers.Add(
                    new KeyValuePair<string, Type>(handlerAttribute.MethodName.ToLower(), handlerType));
            }

            return collectedHandlers;
        }

        internal static MethodInfo GetHandleMethod(IReflect handlerType)
        {
            return handlerType.GetMethod(Constants.HandleMethodName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        }
    }
}