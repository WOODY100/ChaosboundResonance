using System;

namespace Chaosbound.Gameplay.Spawn.Placement.ValueObjects
{
    /// <summary>
    /// Represents the explicit reason why a placement
    /// resolution failed.
    /// </summary>
    public readonly struct FailureReason :
        IEquatable<FailureReason>
    {
        /// <summary>
        /// Gets the unique failure identifier.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new failure reason.
        /// </summary>
        public FailureReason(
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "FailureReason cannot be empty.",
                    nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// No valid placement could be found.
        /// </summary>
        public static FailureReason NoSpaceAvailable =>
            new("spawn.no_space_available");

        /// <summary>
        /// The player is too close to the candidate location.
        /// </summary>
        public static FailureReason PlayerTooClose =>
            new("spawn.player_too_close");

        /// <summary>
        /// The selected spawn point is already occupied.
        /// </summary>
        public static FailureReason SpawnPointOccupied =>
            new("spawn.spawn_point_occupied");

        /// <summary>
        /// The candidate location is outside
        /// the valid world bounds.
        /// </summary>
        public static FailureReason OutsideBounds =>
            new("spawn.outside_bounds");

        /// <summary>
        /// The arena is not currently accepting spawns.
        /// </summary>
        public static FailureReason ArenaClosed =>
            new("spawn.arena_closed");

        public bool Equals(
            FailureReason other)
        {
            return Value == other.Value;
        }

        public override bool Equals(
            object obj)
        {
            return obj is FailureReason other &&
                   Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(
            FailureReason left,
            FailureReason right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            FailureReason left,
            FailureReason right)
        {
            return !left.Equals(right);
        }
    }
}