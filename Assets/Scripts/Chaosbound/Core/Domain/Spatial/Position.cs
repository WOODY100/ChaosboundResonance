using System;

namespace Chaosbound.Core.Domain.Spatial
{
    /// <summary>
    /// Represents a unique location within the expedition space.
    /// </summary>
    public readonly struct Position : IEquatable<Position>
    {
        public float X { get; }

        public float Y { get; }

        public static readonly Position Zero = new(0f, 0f);

        public Position(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Position other)
        {
            return X.Equals(other.X) &&
                   Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            return obj is Position other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}