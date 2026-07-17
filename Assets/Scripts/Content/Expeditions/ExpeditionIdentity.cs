using System;

namespace Chaosbound.Content.Expeditions
{
    /// <summary>
    /// Stable identity of an expedition.
    /// </summary>
    public sealed class ExpeditionIdentity : IEquatable<ExpeditionIdentity>
    {
        public string Id { get; }

        public string DisplayName { get; }

        public ExpeditionIdentity(
            string id,
            string displayName)
        {
            Id = !string.IsNullOrWhiteSpace(id)
                ? id
                : throw new ArgumentException(
                    "Value cannot be null or whitespace.",
                    nameof(id));

            DisplayName = !string.IsNullOrWhiteSpace(displayName)
                ? displayName
                : throw new ArgumentException(
                    "Value cannot be null or whitespace.",
                    nameof(displayName));
        }

        public bool Equals(ExpeditionIdentity other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExpeditionIdentity);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Id);
        }

        public override string ToString()
            => DisplayName;
    }
}