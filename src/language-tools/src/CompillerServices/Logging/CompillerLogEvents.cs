using Microsoft.Extensions.Logging;

namespace CompillerServices.Logging
{
    internal static class CompillerLogEvents
    {
        public static readonly EventId Error = new EventId(-1, "err");
        public static readonly EventId Translate = new EventId(0, "tr");
        public static readonly EventId Build = new EventId(1, "build");
        public static readonly EventId Clean = new EventId(2, "clean");
    }
}