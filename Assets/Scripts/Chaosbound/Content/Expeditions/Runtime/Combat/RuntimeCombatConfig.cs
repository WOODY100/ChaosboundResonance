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
        /// Gets the target progression configured for the expedition.
        /// </summary>
        public RuntimeCombatTargetProgression TargetProgression { get; }

        /// <summary>
        /// Gets the combat tactics available for the expedition.
        /// </summary>
        public IReadOnlyList<RuntimeCombatTactic> Tactics { get; }

        public RuntimeCombatConfig(
            RuntimeCombatTargetProgression targetProgression,
            IReadOnlyList<RuntimeCombatTactic> tactics)
        {
            if (targetProgression == null)
                throw new ArgumentNullException(
                    nameof(targetProgression));

            if (tactics == null)
                throw new ArgumentNullException(nameof(tactics));

            TargetProgression =
                targetProgression;

            Tactics =
                new List<RuntimeCombatTactic>(tactics);
        }
    }
}