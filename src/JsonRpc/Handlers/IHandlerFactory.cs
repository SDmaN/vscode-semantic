namespace JsonRpc.Handlers
{
    public interface IHandlerFactory
    {
        RemoteMethodHandler CreateHandler(string method);
    }
}