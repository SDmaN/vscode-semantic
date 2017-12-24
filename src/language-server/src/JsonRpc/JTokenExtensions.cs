using JsonRpc.Messages;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    internal static class JTokenExtensions
    {
        public static MessageId GetMessageId(this JToken token)
        {
            MessageId result = MessageId.Empty;
            JToken idToken = token[Constants.IdPropertyName];

            if (idToken != null)
            {
                result = idToken.ToObject<MessageId>();
            }

            return result;
        }
    }
}