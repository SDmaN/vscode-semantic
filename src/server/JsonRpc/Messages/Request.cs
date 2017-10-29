namespace JsonRpc.Messages
{
    public class Request : Message, IRequest
    {
        public static readonly IRequest Empty = null;

        public Request(MessageId id, string method, object parameters)
        {
            Id = id;
            Method = method;
            Parameters = parameters;
        }

        public string Method { get; }
        public object Parameters { get; }

        public MessageId Id { get; }
    }

    public sealed class Request<TParameters> : Request, IRequest<TParameters>
    {
        public Request(MessageId id, string method, TParameters parameters)
            : base(id, method, parameters)
        {
            Parameters = parameters;
        }

        public new TParameters Parameters { get; }
        object IRequest.Parameters => Parameters;
    }
}