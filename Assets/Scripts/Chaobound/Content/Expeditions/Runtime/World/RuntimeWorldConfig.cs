using System;
using Chaosbound.Content.World.Themes;
using Chaosbound.Core.Domain.Spatial;

namespace Chaosbound.Content.Expeditions.Runtime.World
{
    /// <summary>
    /// Runtime configuration for the world system.
    /// </summary>
    public sealed class RuntimeWorldConfig
    {
        /// <summary>
        /// Represents an empty runtime configuration.
        /// </summary>
        public static RuntimeWorldConfig Empty { get; } =
            new RuntimeWorldConfig();

        /// <summary>
        /// World boundaries used by runtime systems.
        /// </summary>
        public WorldBounds Bounds { get; }

        /// <summary>
        /// Theme used to build the world.
        /// </summary>
        public WorldThemeAsset Theme { get; }

        private RuntimeWorldConfig()
        {
        }

        public RuntimeWorldConfig(
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