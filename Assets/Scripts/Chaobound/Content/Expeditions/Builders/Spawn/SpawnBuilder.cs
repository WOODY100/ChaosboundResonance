using Chaosbound.Content.Expeditions.Authoring.Spawn;
using Chaosbound.Content.Expeditions.Definitions.Spawn;
using System;

namespace Chaosbound.Content.Expeditions.Builders.Spawn
{
    public static class SpawnBuilder
    {
        public static SpawnDefinition Build(
            SpawnAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            return new SpawnDefinition(
                authoring.Placement,
                authoring.Activation,
                authoring.SpawnConstraints);
        }
    }
}