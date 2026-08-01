using System;

namespace Chaosbound.Gameplay.Spawn.ValueObjects
{
    /// <summary>
    /// Represents the immutable identity of a SpawnJob.
    /// </summary>
    public readonly struct SpawnJobIdentity :
        IEquatable<SpawnJobIdentity>
    {
        public Guid Value { get; }

        public SpawnJobIdentity(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException(
                    "SpawnJobIdentity cannot be empty.",
                    nameof(value));

            Value = value;
        }

        public static SpawnJobIdentity New()
        {
            return new SpawnJobIdentity(Guid.NewGuid());
        }

        public bool Equals(SpawnJobIdentity other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is SpawnJobIdentity other &&
                   Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(
            SpawnJobIdentity left,
            SpawnJobIdentity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            SpawnJobIdentity left,
            SpawnJobIdentity right)
        {
            return !left.Equals(right);
        }
    }
}