using System;

namespace Chaosbound.Gameplay.Spawn.ValueObjects
{
    /// <summary>
    /// Represents the immutable identity of a SpawnTask.
    /// </summary>
    public readonly struct SpawnTaskIdentity :
        IEquatable<SpawnTaskIdentity>
    {
        public Guid Value { get; }

        public SpawnTaskIdentity(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException(
                    "SpawnTaskIdentity cannot be empty.",
                    nameof(value));
            }

            Value = value;
        }

        public static SpawnTaskIdentity New()
        {
            return new SpawnTaskIdentity(Guid.NewGuid());
        }

        public bool Equals(SpawnTaskIdentity other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is SpawnTaskIdentity other &&
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
            SpawnTaskIdentity left,
            SpawnTaskIdentity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            SpawnTaskIdentity left,
            SpawnTaskIdentity right)
        {
            return !left.Equals(right);
        }
    }
}