using System;
using Chaosbound.Content.Expeditions.Authoring.World;
using Chaosbound.Content.Expeditions.Definitions.World;
using Chaosbound.Core.Domain.Spatial;

namespace Chaosbound.Content.Expeditions.Builders.World
{
    /// <summary>
    /// Converts authoring world data into its domain representation.
    /// </summary>
    public static class WorldBuilder
    {
        public static WorldDefinition Build(
    WorldAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            ValidateDimension(
                authoring.Width,
                nameof(authoring.Width));

            ValidateDimension(
                authoring.Height,
                nameof(authoring.Height));

            Position origin = Position.Zero;

            Size size = new Size(
                authoring.Width,
                authoring.Height);

            WorldBounds bounds = new WorldBounds(
                origin,
                size);

            return new WorldDefinition(
                bounds,
                authoring.Theme);
        }

        private static void ValidateDimension(
            int value,
            string parameterName)
        {
            if (value < 3)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "World dimensions must be at least 3.");
            }

            if (value % 2 == 0)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "World dimensions must be odd numbers.");
            }
        }
    }
}