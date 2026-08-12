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
        /// Gets the target progression configured for this expedition.
        /// </summary>
        public CombatTargetProgressionDefinition TargetProgression { get; }

        /// <summary>
        /// Gets the combat tactics configured for this expedition.
        /// </summary>
        public IReadOnlyList<CombatTacticDefinition> Tactics { get; }

        public CombatDefinition(
            CombatTargetProgressionDefinition targetProgression,
            IReadOnlyList<CombatTacticDefinition> tactics)
        {
            if (targetProgression == null)
                throw new ArgumentNullException(
                    nameof(targetProgression));

            if (tactics == null)
                throw new ArgumentNullException(
                    nameof(tactics));

            TargetProgression =
                targetProgression;

            Tactics =
                new List<CombatTacticDefinition>(tactics);
        }
    }
}