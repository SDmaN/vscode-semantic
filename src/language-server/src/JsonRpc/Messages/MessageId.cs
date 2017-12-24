using System;
using JsonRpc.Converters;
using Newtonsoft.Json;

namespace JsonRpc.Messages
{
    [JsonConverter(typeof(MessageIdConverter))]
    public struct MessageId : IEquatable<MessageId>
    {
        public const string LongValueFieldName = nameof(_longValue);
        public const string StringValueFieldName = nameof(_stringValue);

        private readonly long? _longValue;
        private readonly string _stringValue;

        public static readonly MessageId Empty = new MessageId(null);

        public MessageId(long value)
        {
            _longValue = value;
            _stringValue = null;
        }

        public MessageId(string value)
        {
            _longValue = null;
            _stringValue = value;
        }

        public override string ToString()
        {
            return _longValue != null ? _longValue.Value.ToString() : _stringValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is MessageId id && Equals(id);
        }

        public bool Equals(MessageId other)
        {
            return _longValue == other._longValue && string.Equals(_stringValue, other._stringValue);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_longValue?.GetHashCode() ?? 0 * 397) ^
                       (_stringValue != null ? _stringValue.GetHashCode() : 0);
            }
        }

        public static bool operator ==(MessageId left, MessageId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MessageId left, MessageId right)
        {
            return !(left == right);
        }

        public static explicit operator long(MessageId id)
        {
            if (id._longValue == null)
            {
                throw new InvalidOperationException("Id is not a number.");
            }

            return id._longValue.Value;
        }
    }
}