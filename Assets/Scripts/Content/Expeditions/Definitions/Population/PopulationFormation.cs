using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions
{
    /// <summary>
    /// Defines a combat formation available to an expedition.
    /// </summary>
    public sealed class PopulationFormation
    {
        /// <summary>
        /// Units composing this formation.
        /// </summary>
        public IReadOnlyList<CombatFormationEntry> Entries { get; }

        public PopulationFormation(
            IReadOnlyList<CombatFormationEntry> entries)
        {
            Entries = entries ??
                throw new ArgumentNullException(nameof(entries));
        }
    }
}