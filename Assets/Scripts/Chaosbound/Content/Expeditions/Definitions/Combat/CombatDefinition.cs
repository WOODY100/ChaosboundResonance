using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Combat
{
    /// <summary>
    /// Defines the combat configuration available for an expedition.
    /// </summary>
    public sealed class CombatDefinition
    {
        /// <summary>
        /// Gets the combat tactics configured for this expedition.
        /// </summary>
        public IReadOnlyList<CombatTacticDefinition> Tactics { get; }

        public CombatDefinition(
            IReadOnlyList<CombatTacticDefinition> tactics)
        {
            if (tactics == null)
                throw new ArgumentNullException(nameof(tactics));

            Tactics = new List<CombatTacticDefinition>(tactics);
        }
    }
}