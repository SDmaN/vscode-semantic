using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Exceptions;
using JsonRpc.Handlers;
using JsonRpc.HandleResult;
using JsonRpc.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static JsonRpc.Constants;

namespace JsonRpc
{
    public class RpcService : IRpcService
    {
        private readonly IHandlerFactory _handlerFactory;
        private readonly IRequestCancellationManager _cancellationManager;

        public RpcService(IHandlerFactory handlerFactory, IRequestCancellationManager cancellationManager)
        {
            _handlerFactory = handlerFactory;
            _cancellationManager = cancellationManager;
        }

        public async Task HandleRequest(IInput input, IOutput output, CancellationToken cancellationToken = default)
        {
            JToken request;

            try
            {
                request = await input.ReadAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await output.WriteAsync(
                    JToken.FromObject(Response.CreateParseError(new MessageId(null),
                        $"Could not parse request: {e.Message}")),
                    cancellationToken);

                return;
            }
            
            Task task = new Task(async () =>
            {
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
            });

            task.Start();
        }

        private async Task<IResponse> HandleRequestObject(JObject requestObject)
        {
            MessageId id = MessageId.Empty;

            try
            {
                id = ParseMessageId(requestObject);

                _cancellationManager.AddCancellation(id);

                RemoteMethodHandler handler = GetHandler(id, requestObject);

                MethodInfo handleMethod = HandlerHelper.GetHandleMethod(handler.GetType());
                ParameterInfo[] handlerParameters = handleMethod.GetParameters();

                JToken paramsToken = requestObject[ParamsPropertyName];
                object[] handleParameterValues = await GetHandlerParameterValues(paramsToken, handlerParameters);
                _cancellationManager.EnsureCancelled(id);

                IResponse response = await CallHandler(id, handler, handleMethod, handleParameterValues);
                _cancellationManager.EnsureCancelled(id);

                _cancellationManager.RemoveCancellation(id);

                return response;
            }
            catch (JsonSerializationException)
            {
                _cancellationManager.RemoveCancellation(id);
                return Response.CreateParseErrorOrNull(id, "JSON parsing error occured.");
            }
            catch (RequestWithIdAlreadyExistsException e)
            {
                _cancellationManager.RemoveCancellation(id);
                return Response.CreateInvalidRequestErrorOrNull(id, e.Message);
            }
            catch (HandleMethodNotSpecifiedException e)
            {
                _cancellationManager.RemoveCancellation(id);
                return Response.CreateInvalidRequestErrorOrNull(id, e.Message);
            }
            catch (ParameterException e)
            {
                _cancellationManager.RemoveCancellation(id);
                return Response.CreateInvalidParamsErrorOrNull(id, e.Message);
            }
            catch (TargetInvocationException e)
            {
                _cancellationManager.RemoveCancellation(id);
                return Response.CreateInternalErrorOrNull(id, e.InnerException.Message);
            }
            catch (TaskCanceledException)
            {
                _cancellationManager.RemoveCancellation(id);
                return Response.CreateRequestCancelledErrorOrNull(id, string.Empty);
            }
            catch (Exception e)
            {
                _cancellationManager.RemoveCancellation(id);
                return Response.CreateInternalErrorOrNull(id, e.Message);
            }
        }

        private RemoteMethodHandler GetHandler(MessageId id, JObject requestObject)
        {
            string method = requestObject[MethodPropertyName]?.ToString();

            if (string.IsNullOrWhiteSpace(method))
            {
                throw new HandleMethodNotSpecifiedException("Method not specified.");
            }

            RemoteMethodHandler handler = _handlerFactory.CreateHandler(method.ToLower());

            handler.Request = requestObject.ToObject<Request>();
            handler.CancellationToken = _cancellationManager.GetToken(id);

            return handler;
        }

        private static async Task<object[]> GetHandlerParameterValues(JToken paramsToken,
            IReadOnlyList<ParameterInfo> handlerParameters)
        {
            object[] handlerParameterValues = null;

            if (paramsToken != null && paramsToken.Type != JTokenType.Null)
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
                        throw new InvalidParamsPropertyException("Params is not an object or array.");
                }
            }

            return handlerParameterValues;
        }

        private static Task<object[]> ConvertArrayParams(JArray paramsArray,
            IReadOnlyList<ParameterInfo> handlerParameters)
        {
            if (paramsArray.Count != handlerParameters.Count)
            {
                throw new ParametersCountMismatchException(
                    $"Parameters count mismatch. Handler method has {handlerParameters.Count} and request has {paramsArray.Count}");
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
                throw new ParametersCountMismatchException(
                    $"Parameters count mismatch. Handler method has {handlerParameters.Count} and request has {paramsObject.Count}");
            }

            object[] result = new object[paramsObject.Count];

            foreach (JProperty paramsProperty in paramsObject.Properties())
            {
                ParameterInfo parameter = SearchParameter(paramsProperty.Name, handlerParameters);

                if (parameter == null)
                {
                    throw new ParameterNotFoundException(paramsProperty.Name,
                        $"Parameter {paramsProperty.Name} not found.");
                }

                result[parameter.Position] = paramsProperty.Value.ToObject(parameter.ParameterType);
            }

            return Task.FromResult(result);
        }

        private static async Task<IResponse> CallHandler(MessageId id, object handler, MethodInfo handlerMethod,
            object[] handlerParameters)
        {
            try
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
            catch (ArgumentException)
            {
                throw new ParameterException("Parameters mismatch.");
            }
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
                    throw new HandlerNotAnsweredForRequestException("Handler not answered for request.");

                case IRpcHandleResult rpcHandleResult:
                    return rpcHandleResult.GetResponse(id);

                default:
                    return new Response(id, callResult);
            }
        }

        private static ParameterInfo SearchParameter(string name, IEnumerable<ParameterInfo> parameters)
        {
            return parameters.FirstOrDefault(
                x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        private static MessageId ParseMessageId(JToken token)
        {
            MessageId result = MessageId.Empty;
            JToken idToken = token[IdPropertyName];

            if (idToken != null)
            {
                result = idToken.ToObject<MessageId>();
            }

            return result;
        }
    }
}