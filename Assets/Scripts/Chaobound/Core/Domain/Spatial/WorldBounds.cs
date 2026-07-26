using System;

namespace Chaosbound.Core.Domain.Spatial
{
    /// <summary>
    /// Represents the absolute spatial limits of an expedition.
    /// </summary>
    public sealed class WorldBounds : IEquatable<WorldBounds>
    {
        public Position Origin { get; }

        public Size Size { get; }

        public Position Center =>
            new Position(
                Origin.X + (Size.Width * 0.5f),
                Origin.Y + (Size.Depth * 0.5f));

        public WorldBounds(Position origin, Size size)
        {
            Origin = origin;
            Size = size;
        }

        public bool Contains(Position position)
        {
            return position.X >= Origin.X &&
                   position.X <= Origin.X + Size.Width &&
                   position.Y >= Origin.Y &&
                   position.Y <= Origin.Y + Size.Depth;
        }

        public Position Clamp(Position position)
        {
            float x = Math.Clamp(
                position.X,
                Origin.X,
                Origin.X + Size.Width);

            float y = Math.Clamp(
                position.Y,
                Origin.Y,
                Origin.Y + Size.Depth);

            return new Position(x, y);
        }

        public bool Equals(WorldBounds other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Origin.Equals(other.Origin)
                && Size.Equals(other.Size);
        }

        public override bool Equals(object obj)
        {
            return obj is WorldBounds other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Origin, Size);
        }

        public static bool operator ==(WorldBounds left, WorldBounds right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WorldBounds left, WorldBounds right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"Origin: {Origin} | Size: {Size}";
        }
    }
}