namespace JsonRpc.Messages
{
    public enum ErrorCode
    {
        ParseError = -32700,
        InvalidRequest = -32600,
        MethodNotFound = -32601,
        InvalidParams = -32602,
        InternalError = -32603,
        ServerNotInitialzed = -32002,
        RequestCanceled = -32800
    }
}