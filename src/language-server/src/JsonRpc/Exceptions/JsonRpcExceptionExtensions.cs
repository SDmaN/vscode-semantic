using System;
using JsonRpc.Messages;

namespace JsonRpc.Exceptions
{
    internal static class JsonRpcExceptionExtensions
    {
        public static IResponse CreateResponse(this JsonRpcException exception, MessageId id)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            switch (exception)
            {
                case RequestWithIdAlreadyExistsException idEx:
                    return Response.CreateInvalidRequestErrorOrNull(id, idEx.Message);

                case HandleMethodNotSpecifiedException methodNotSpecifiedException:
                    return Response.CreateInvalidRequestErrorOrNull(id, methodNotSpecifiedException.Message);

                case HandlerNotFoundException handlerNotFoundException:
                    return Response.CreateInternalErrorOrNull(id, handlerNotFoundException.Message);

                case ParameterException parameterException:
                    return Response.CreateInvalidParamsErrorOrNull(id, parameterException.Message);

                default:
                    throw new ArgumentOutOfRangeException(nameof(exception));
            }
        }
    }
}