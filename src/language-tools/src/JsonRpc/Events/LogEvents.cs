using Microsoft.Extensions.Logging;

namespace JsonRpc.Events
{
    internal static class LogEvents
    {
        public static readonly EventId CouldNotParseRequestEventId = new EventId(-1, "COULD_NOT_PARSE_REQUEST");
        public static readonly EventId JsonSerializationErrorEventId = new EventId(-2, "JSON_SERIALIZATION_ERROR");
        public static readonly EventId JsonRpcErrorEventId = new EventId(-3, "JSON_RPC_ERROR");
        public static readonly EventId HandlerInvocationErrorEventId = new EventId(-4, "HANDLER_INVOCATION_ERROR");
        public static readonly EventId UnknownRpcErrorEventId = new EventId(-5, "UNKNOWN_RPC_ERROR");

        public static readonly EventId IncommingMessageEventId = new EventId(0, "INCOMMING_MESSAGE");
        public static readonly EventId HandlingRequestWithEventId = new EventId(1, "HANDLING_REQUEST");
        public static readonly EventId RequestCancelledEventId = new EventId(2, "REQUEST_CANCELLED");
        public static readonly EventId OutgoingResponseEventId = new EventId(3, "OUTGOING_RESPONSE");
    }
}