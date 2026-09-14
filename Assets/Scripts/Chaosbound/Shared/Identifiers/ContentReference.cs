using System;

namespace Chaosbound.Shared.Identifiers
{
    /// <summary>
    /// Represents an immutable reference to authored expedition content.
    /// </summary>
    public sealed class ContentReference :
        IEquatable<ContentReference>
    {
        public ContentId Id { get; }

        public ContentReference(
            ContentId id)
        {
            if (id.IsEmpty)
                throw new ArgumentException(
                    "Content reference requires a valid content identifier.",
                    nameof(id));

            Id = id;
        }

        public bool Equals(
            ContentReference other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override bool Equals(
            object obj)
        {
            return obj is ContentReference other &&
                   Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public static bool operator ==(
            ContentReference left,
            ContentReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(
            ContentReference left,
            ContentReference right)
        {
            return !Equals(left, right);
        }
    }
}