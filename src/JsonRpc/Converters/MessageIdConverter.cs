using System;
using JsonRpc.Messages;
using Newtonsoft.Json;

namespace JsonRpc.Converters
{
    public class MessageIdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!CanConvert(value.GetType()))
            {
                throw new InvalidCastException($"Couldn't convert {value.GetType().Name} to {typeof(MessageId).Name}.");
            }

            try
            {
                long longValue = (long) (MessageId) value;
                writer.WriteValue(longValue);
            }
            catch (InvalidOperationException)
            {
                writer.WriteValue(value.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    return new MessageId((long) reader.Value);
                case JsonToken.String:
                    return new MessageId((string) reader.Value);
                case JsonToken.Null:
                    return new MessageId(null);
                default:
                    throw new JsonException("Json parsing error.");
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MessageId);
        }
    }
}