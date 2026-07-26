using System;

namespace Chaosbound.Shared.Identifiers
{
    /// <summary>
    /// Represents the stable identifier of any authored content in Chaosbound.
    /// </summary>
    public readonly struct ContentId : IEquatable<ContentId>
    {
        private readonly string value;

        /// <summary>
        /// Gets the identifier value.
        /// </summary>
        public string Value => value;

        /// <summary>
        /// Creates a new content identifier.
        /// </summary>
        /// <param name="value">Stable content identifier.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the identifier is null, empty or whitespace.
        /// </exception>

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
            return value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return value;
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

        public static implicit operator string(ContentId id)
        {
            return id.value;
        }

        public static explicit operator ContentId(string value)
        {
            return new ContentId(value);
        }
    }
}