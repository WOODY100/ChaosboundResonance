using Chaosbound.Content.Expeditions.Definitions.Combat;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Combat
{
    /// <summary>
    /// Immutable runtime configuration containing the combat
    /// configuration available for the current expedition.
    /// </summary>
    public sealed class RuntimeCombatConfig
    {
        /// <summary>
        /// Gets the combat tactics available for the expedition.
        /// </summary>
        public IReadOnlyList<CombatTacticDefinition> Tactics { get; }

        public RuntimeCombatConfig(
            IReadOnlyList<CombatTacticDefinition> tactics)
        {
            if (tactics == null)
                throw new ArgumentNullException(nameof(tactics));

            Tactics = new List<CombatTacticDefinition>(tactics);
        }
    }
}