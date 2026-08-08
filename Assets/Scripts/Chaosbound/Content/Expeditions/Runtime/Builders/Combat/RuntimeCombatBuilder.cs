using Chaosbound.Content.Expeditions.Definitions.Combat;
using Chaosbound.Content.Expeditions.Runtime.Combat;
using System;

namespace Chaosbound.Content.Expeditions.Runtime.Builders
{
    /// <summary>
    /// Builds the runtime combat configuration from
    /// the immutable combat definition.
    /// </summary>
    public sealed class RuntimeCombatBuilder
    {
        public RuntimeCombatConfig BuildCombat(
            CombatDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new RuntimeCombatConfig(
                definition.Tactics);
        }
    }
}