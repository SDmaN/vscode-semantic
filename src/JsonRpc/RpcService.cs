using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Exceptions;
using JsonRpc.HandleResult;
using JsonRpc.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static JsonRpc.Constants;

namespace JsonRpc
{
    public class RpcService : IRpcService
    {
        private readonly IDictionary<string, object> _handlers = new Dictionary<string, object>();

        public void RegisterHandler(object handler)
        {
            Type handlerType = handler.GetType();
            RemoteMethodHandlerAttribute handlerAttribute =
                handler.GetType().GetCustomAttribute<RemoteMethodHandlerAttribute>();

            if (handlerAttribute == null)
            {
                throw new NotHandlerRegisteringException(handlerType,
                    $"Registering type is not marked with {typeof(RemoteMethodHandlerAttribute).Name} attribute.");
            }

            MethodInfo handleMethod = GetHandleMethod(handlerType);

            if (handleMethod == null)
            {
                throw new HandleMethodNotFoundException(handlerAttribute.MethodName, handlerType,
                    $"Method {HandleMethodName} not found in {handlerType.Name}.");
            }

            _handlers.Add(handlerAttribute.MethodName.ToLower(), handler);
        }

        public async Task HandleRequest(IInput input, IOutput output, CancellationToken cancellationToken = default)
        {
            JToken request;

            try
            {
                request = await input.ReadAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await output.WriteAsync(
                    JToken.FromObject(Response.CreateParseError(new MessageId(null),
                        $"Could not parse request: {e.Message}")),
                    cancellationToken);

                return;
            }
            
            JToken responseToken = null;

            switch (request)
            {
                case JArray requestArray:
                    JArray responseArray = new JArray();

                    foreach (JToken jToken in requestArray)
                    {
                        if (jToken.Type == JTokenType.Object)
                        {
                            IResponse response = await HandleRequestObject(jToken as JObject);

                            if (response != null)
                            {
                                responseArray.Add(JObject.FromObject(response));
                            }
                        }
                    }

                    responseToken = responseArray;
                    break;

                case JObject requestObject:
                    IResponse responseObject = await HandleRequestObject(requestObject);

                    if (responseObject != null)
                    {
                        responseToken = JToken.FromObject(responseObject);
                    }
                    break;

                default:
                    responseToken = JToken.FromObject(Response.CreateParseError(new MessageId(null),
                        "Request could not be parsed."));
                    break;
            }

            if (responseToken != null)
            {
                await output.WriteAsync(responseToken, cancellationToken);
            }
        }

        private async Task<IResponse> HandleRequestObject(JObject requestObject)
        {
            JToken idToken = requestObject[IdPropertyName];
            MessageId id = MessageId.Empty;

            try
            {
                if (idToken != null)
                {
                    id = idToken.ToObject<MessageId>();
                }

                string method = requestObject[MethodPropertyName]?.ToString();

                if (method == null && id != MessageId.Empty)
                {
                    return Response.CreateInvalidParamsErrorOrNull(id, $"Method not specified");
                }

                if (!_handlers.TryGetValue(method.ToLower(), out object handler))
                {
                    return Response.CreateMethodNotFoundErrorOrNull(id, $"Method {method} not exists.");
                }

                MethodInfo handleMethod = GetHandleMethod(handler.GetType());
                ParameterInfo[] handlerParameters = handleMethod.GetParameters();

                JToken paramsToken = requestObject[ParamsPropertyName];
                object[] handleParameterValues = await GetHandlerParameterValues(paramsToken, handlerParameters);

                return await CallHandler(id, handler, handleMethod, handleParameterValues);
            }
            catch (JsonSerializationException)
            {
                return Response.CreateParseError(new MessageId(null), "JSON parsing error occured.");
            }
            catch (ParametersCountMismatchException)
            {
                return Response.CreateInvalidParamsErrorOrNull(id, $"Parameters count mismatch.");
            }
            catch (ParameterNotFoundException e)
            {
                return Response.CreateInvalidParamsErrorOrNull(id,
                    $"Parameter {e.ParamName} is not a method parameter.");
            }
            catch (InvalidParamsPropertyException)
            {
                return Response.CreateInvalidParamsErrorOrNull(id, "Params property is not an object or array.");
            }
            catch (ArgumentException)
            {
                return Response.CreateInvalidParamsErrorOrNull(id, "Parameters mismatch.");
            }
            catch (TargetInvocationException e)
            {
                return Response.CreateInternalErrorOrNull(id, e.InnerException.Message);
            }
            catch (Exception e)
            {
                return Response.CreateInternalErrorOrNull(id, e.Message);
            }
        }

        private static async Task<object[]> GetHandlerParameterValues(JToken paramsToken,
            IReadOnlyList<ParameterInfo> handlerParameters)
        {
            object[] handlerParameterValues = null;

            if (paramsToken != null)
            {
                switch (paramsToken)
                {
                    case JArray paramsArray:
                        handlerParameterValues = await ConvertArrayParams(paramsArray, handlerParameters);
                        break;

                    case JObject paramsObject:
                        handlerParameterValues = await ConvertObjectParams(paramsObject, handlerParameters);
                        break;

                    default:
                        throw new InvalidParamsPropertyException();
                }
            }

            return handlerParameterValues;
        }

        private static Task<object[]> ConvertArrayParams(JArray paramsArray,
            IReadOnlyList<ParameterInfo> handlerParameters)
        {
            if (paramsArray.Count != handlerParameters.Count)
            {
                throw new ParametersCountMismatchException();
            }

            object[] result = new object[paramsArray.Count];

            for (int i = 0; i < paramsArray.Count; i++)
            {
                object parameter = paramsArray[i].ToObject(handlerParameters[i].ParameterType);
                result[i] = parameter;
            }

            return Task.FromResult(result);
        }

        private static Task<object[]> ConvertObjectParams(JObject paramsObject,
            IReadOnlyCollection<ParameterInfo> handlerParameters)
        {
            if (paramsObject.Count != handlerParameters.Count)
            {
                throw new ParametersCountMismatchException();
            }

            object[] result = new object[paramsObject.Count];

            foreach (JProperty paramsProperty in paramsObject.Properties())
            {
                ParameterInfo parameter = SearchParameter(paramsProperty.Name, handlerParameters);

                if (parameter == null)
                {
                    throw new ParameterNotFoundException(paramsProperty.Name);
                }

                result[parameter.Position] = paramsProperty.Value.ToObject(parameter.ParameterType);
            }

            return Task.FromResult(result);
        }

        private static async Task<IResponse> CallHandler(MessageId id, object handler, MethodInfo handlerMethod,
            object[] handlerParameters)
        {
            if (!typeof(Task).IsAssignableFrom(handlerMethod.ReturnType))
            {
                object callResult = handlerMethod.Invoke(handler, handlerParameters);
                return GetResponseFromCallResult(id, callResult);
            }

            Task callTask = (Task) handlerMethod.Invoke(handler, handlerParameters);
            await callTask;

            if (!callTask.GetType().IsGenericType)
            {
                return null;
            }

            PropertyInfo resultProperty = callTask.GetType().GetProperty("Result");
            return GetResponseFromCallResult(id, resultProperty.GetValue(callTask));
        }

        private static IResponse GetResponseFromCallResult(MessageId id, object callResult)
        {
            if (id == MessageId.Empty)
            {
                return null;
            }

            switch (callResult)
            {
                case null:
                    throw new HandlerNotAnsweredForRequest("Handler not answered for request.");

                case IRpcHandleResult rpcHandleResult:
                    return rpcHandleResult.GetResponse(id);

                default:
                    return new Response(id, callResult);
            }
        }

        private static MethodInfo GetHandleMethod(IReflect handlerType)
        {
            return handlerType.GetMethod(HandleMethodName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        }

        private static ParameterInfo SearchParameter(string name, IEnumerable<ParameterInfo> parameters)
        {
            return parameters.FirstOrDefault(
                x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}