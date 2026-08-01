using System;

namespace Chaosbound.Shared.Identifiers
{
    /// <summary>
    /// Represents the stable identifier of any authored content in Chaosbound.
    /// </summary>
    public readonly struct ContentId : IEquatable<ContentId>
    {
        private readonly string value;

        public string Value
        {
            get
            {
                return value;
            }
        }

        public static readonly ContentId Empty = default;

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(value);
            }
        }

        public ContentId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(
                    "ContentId cannot be null or empty.",
                    nameof(value));

            this.value = value;
        }

        public bool Equals(ContentId other)
        {
            return string.Equals(
                value,
                other.value,
                StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is ContentId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(value);
        }

        public override string ToString()
        {
            return value ?? string.Empty;
        }

        public static bool operator ==(
            ContentId left,
            ContentId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            ContentId left,
            ContentId right)
        {
            return !left.Equals(right);
        }

        public static explicit operator ContentId(string value)
        {
            return new ContentId(value);
        }
    }
}