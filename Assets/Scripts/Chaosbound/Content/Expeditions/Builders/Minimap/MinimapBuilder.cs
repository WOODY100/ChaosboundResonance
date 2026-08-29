using Chaosbound.Content.Expeditions.Authoring.Minimap;
using Chaosbound.Content.Expeditions.Definitions.Minimap;
using System;

namespace Chaosbound.Content.Expeditions.Builders.Minimap
{
    public static class MinimapBuilder
    {
        public static MinimapDefinition Build(
            MinimapAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(
                    nameof(authoring));

            return new MinimapDefinition(
                authoring.WalkableTexture);
        }
    }
}