using System;
using Chaosbound.Core.Domain.Spatial;

namespace Chaosbound.Content.Expeditions.Definitions
{
    /// <summary>
    /// Describes the world properties of an expedition.
    /// </summary>
    public sealed class WorldDefinition
    {
        public WorldBounds Bounds { get; }

        public WorldDefinition(
            WorldBounds bounds)
        {
            Bounds = bounds ??
                throw new ArgumentNullException(nameof(bounds));
        }
    }
}