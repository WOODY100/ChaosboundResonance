using System;

namespace Chaosbound.Gameplay.Spawn.ValueObjects
{
    /// <summary>
    /// Represents the immutable identity of a SpawnRequest.
    /// </summary>
    public readonly struct SpawnRequestId :
        IEquatable<SpawnRequestId>
    {
        /// <summary>
        /// Gets the underlying Guid value.
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Creates a new spawn request identifier.
        /// </summary>
        /// <param name="value">
        /// Guid value.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when the Guid is empty.
        /// </exception>
        public SpawnRequestId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException(
                    "SpawnRequestId cannot be empty.",
                    nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// Creates a new unique spawn request identifier.
        /// </summary>
        public static SpawnRequestId New()
        {
            return new SpawnRequestId(Guid.NewGuid());
        }

        /// <inheritdoc/>
        public bool Equals(SpawnRequestId other)
        {
            return Value.Equals(other.Value);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is SpawnRequestId other &&
                   Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(
            SpawnRequestId left,
            SpawnRequestId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            SpawnRequestId left,
            SpawnRequestId right)
        {
            return !left.Equals(right);
        }
    }
}