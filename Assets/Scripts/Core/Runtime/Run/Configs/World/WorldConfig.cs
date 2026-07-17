using System;
using Chaosbound.Core.Domain.Spatial;

namespace Chaosbound.Runtime.Run.Configs.World
{
    /// <summary>
    /// Runtime configuration for the world system.
    /// </summary>
    public sealed class WorldConfig
    {
        /// <summary>
        /// Represents an empty runtime configuration.
        /// </summary>
        public static WorldConfig Empty { get; } =
            new WorldConfig();

        public WorldBounds Bounds { get; }

        private WorldConfig()
        {
        }

        public WorldConfig(
            WorldBounds bounds)
        {
            Bounds = bounds ??
                throw new ArgumentNullException(nameof(bounds));
        }
    }
}