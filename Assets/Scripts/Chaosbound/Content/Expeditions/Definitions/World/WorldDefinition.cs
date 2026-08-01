using System;
using Chaosbound.Content.World.Themes;
using Chaosbound.Core.Domain.Spatial;

namespace Chaosbound.Content.Expeditions.Definitions.World
{
    /// <summary>
    /// Describes the world properties of an expedition.
    /// </summary>
    public sealed class WorldDefinition
    {
        public WorldBounds Bounds { get; }

        public WorldThemeAsset Theme { get; }

        public WorldDefinition(
            WorldBounds bounds,
            WorldThemeAsset theme)
        {
            Bounds = bounds ??
                throw new ArgumentNullException(nameof(bounds));

            Theme = theme ??
                throw new ArgumentNullException(nameof(theme));
        }
    }
}