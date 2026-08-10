using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Models
{
    /// <summary>
    /// Represents the concrete materialization plan produced
    /// by the Combat domain.
    ///
    /// This plan describes which enemy variants should be
    /// materialized and how many of each are required.
    ///
    /// It does not contain spawn positions, spawn points,
    /// placement information, or Spawn Runtime behavior.
    /// </summary>
    public sealed class CombatSpawnPlan
    {
        private readonly List<CombatSpawnPlanEntry>
            entries;

        /// <summary>
        /// Gets the concrete materialization entries.
        /// </summary>
        public IReadOnlyList<CombatSpawnPlanEntry>
            Entries =>
            entries;

        /// <summary>
        /// Gets whether this plan contains no entries.
        /// </summary>
        public bool IsEmpty =>
            entries.Count == 0;

        /// <summary>
        /// Gets the total number of enemies requested
        /// by this plan.
        /// </summary>
        public int TotalQuantity
        {
            get
            {
                int total = 0;

                foreach (
                    CombatSpawnPlanEntry entry
                    in entries)
                {
                    total += entry.Quantity;
                }

                return total;
            }
        }

        public CombatSpawnPlan(
            IReadOnlyList<CombatSpawnPlanEntry> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(
                    nameof(entries));
            }

            this.entries =
                new List<CombatSpawnPlanEntry>(
                    entries.Count);

            foreach (
                CombatSpawnPlanEntry entry
                in entries)
            {
                if (entry == null)
                {
                    throw new InvalidOperationException(
                        "CombatSpawnPlan contains a null entry.");
                }

                this.entries.Add(entry);
            }
        }
    }
}