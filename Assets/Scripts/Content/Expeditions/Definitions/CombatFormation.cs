using Chaosbound.Runtime.Population;
using Chaosbound.Content.Expeditions.Definitions;
using System;
using System.Collections.Generic;

namespace Chaosbound.Runtime.Run.Configs.Population
{
    /// <summary>
    /// Represents a tactical composition of combat roles.
    /// </summary>
    public sealed class CombatFormation
    {
        /// <summary>
        /// Tactical requirements of the formation.
        /// </summary>
        public IReadOnlyList<CombatFormationEntry> Entries { get; }

        public CombatFormation(
            IReadOnlyList<CombatFormationEntry> entries)
        {
            Entries = entries ??
                throw new ArgumentNullException(nameof(entries));
        }
    }
}