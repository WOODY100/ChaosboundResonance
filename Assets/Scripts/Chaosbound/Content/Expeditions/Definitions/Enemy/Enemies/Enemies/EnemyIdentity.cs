using System;

namespace Chaosbound.Content.Expeditions.Domain.Definitions.Enemy.Enemies
{
    /// <summary>
    /// Stable identity of an enemy.
    /// </summary>
    public sealed class EnemyIdentity :
        IEquatable<EnemyIdentity>
    {
        public string Id { get; }

        public string DisplayName { get; }

        public EnemyIdentity(
            string id,
            string displayName)
        {
            Id = !string.IsNullOrWhiteSpace(id)
                ? id
                : throw new ArgumentException(nameof(id));

            DisplayName = !string.IsNullOrWhiteSpace(displayName)
                ? displayName
                : throw new ArgumentException(nameof(displayName));
        }

        public bool Equals(EnemyIdentity other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EnemyIdentity);
        }

        public override int GetHashCode()
            => Id.GetHashCode();

        public override string ToString()
            => DisplayName;
    }
}