namespace JsonRpc.Messages
{
    public class Response : Message, IResponse
    {
        public Response(MessageId id, object result)
        {
            Id = id;
            Result = result;
            Error = null;
        }

        public Response(Request request, object result)
            : this(request.Id, result)
        {
        }

        public Response(MessageId id, Error error)
        {
            Id = id;
            Result = null;
            Error = error;
        }

        public Response(Request request, Error error)
            : this(request.Id, error)
        {
        }

        public MessageId Id { get; }

        public object Result { get; }
        public Error Error { get; }

        public static Response CreateInvalidParamsError(MessageId id, string message)
        {
            return new Response(id, new Error(ErrorCode.InvalidParams, message));
        }

        public static Response CreateInvalidParamsErrorOrNull(MessageId id, string message)
        {
            return id == MessageId.Empty ? null : CreateInvalidParamsError(id, message);
        }

        public static Response CreateParseError(MessageId id, string message)
        {
            return new Response(id, new Error(ErrorCode.ParseError, message));
        }

        public static Response CreateMethodNotFoundError(MessageId id, string message)
        {
            return new Response(id, new Error(ErrorCode.MethodNotFound, message));
        }

        public static Response CreateMethodNotFoundErrorOrNull(MessageId id, string message)
        {
            return id == MessageId.Empty ? null : CreateMethodNotFoundError(id, message);
        }

        public static Response CreateInvalidRequestError(MessageId id, string message)
        {
            return new Response(id, new Error(ErrorCode.InvalidRequest, message));
        }

        public static Response CreateInvalidRequestErrorOrNull(MessageId id, string message)
        {
            return id == MessageId.Empty ? null : CreateInvalidRequestError(id, message);
        }

        public static Response CreateInternalError(MessageId id, string message)
        {
            return new Response(id, new Error(ErrorCode.InternalError, message));
        }

        public static Response CreateInternalErrorOrNull(MessageId id, string message)
        {
            return id == MessageId.Empty ? null : CreateInternalError(id, message);
        }

        public static Response CreateRequestCancelledError(MessageId id, string message)
        {
            return new Response(id, new Error(ErrorCode.RequestCanceled, message));
        }

        public static Response CreateRequestCancelledErrorOrNull(MessageId id, string message)
        {
            return id == MessageId.Empty ? null : CreateRequestCancelledError(id, message);
        }
    }

    public sealed class Response<TResult> : Response, IResponse<TResult> where TResult : class
    {
        public Response(MessageId id, TResult result)
            : base(id, result)
        {
            Result = result;
        }

        public Response(Request request, TResult result)
            : this(request.Id, result)
        {
        }

        public Response(MessageId id, Error error)
            : base(id, error)
        {
            Result = null;
        }

        public Response(Request request, Error error)
            : this(request.Id, error)
        {
        }

        public new TResult Result { get; }
        object IResponse.Result => Result;
    }
}