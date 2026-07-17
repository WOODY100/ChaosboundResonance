using System;

namespace Chaosbound.Core.Domain.Spatial
{
    /// <summary>
    /// Represents the dimensions of a spatial element.
    /// </summary>
    public readonly struct Size : IEquatable<Size>
    {
        public float Width { get; }

        public float Depth { get; }

        public Size(float width, float depth)
        {
            if (width <= 0f)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (depth <= 0f)
                throw new ArgumentOutOfRangeException(nameof(depth));

            Width = width;
            Depth = depth;
        }

        public bool Equals(Size other)
        {
            return Width.Equals(other.Width) &&
                   Depth.Equals(other.Depth);
        }

        public override bool Equals(object obj)
        {
            return obj is Size other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Depth);
        }

        public static bool operator ==(Size left, Size right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Size left, Size right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({Width} x {Depth})";
        }
    }
}