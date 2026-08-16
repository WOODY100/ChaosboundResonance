using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Bosses.Models
{
    /// <summary>
    /// Represents the declarative materialization plan
    /// produced by the Boss Domain.
    /// </summary>
    public sealed class BossSpawnPlan
    {
        private readonly List<BossSpawnPlanEntry>
            entries;

        /// <summary>
        /// Gets the Boss materialization entries.
        /// </summary>
        public IReadOnlyList<BossSpawnPlanEntry>
            Entries =>
            entries;

        /// <summary>
        /// Gets whether this plan contains no entries.
        /// </summary>
        public bool IsEmpty =>
            entries.Count == 0;

        /// <summary>
        /// Gets the total number of Boss entities
        /// requested by this plan.
        /// </summary>
        public int TotalQuantity
        {
            get
            {
                int total = 0;

                foreach (
                    BossSpawnPlanEntry entry
                    in entries)
                {
                    total += entry.Quantity;
                }

                return total;
            }
        }

        public BossSpawnPlan(
            IReadOnlyList<BossSpawnPlanEntry> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(
                    nameof(entries));
            }

            this.entries =
                new List<BossSpawnPlanEntry>(
                    entries.Count);

            foreach (
                BossSpawnPlanEntry entry
                in entries)
            {
                if (entry == null)
                {
                    throw new InvalidOperationException(
                        "BossSpawnPlan contains a null entry.");
                }

                this.entries.Add(entry);
            }
        }
    }
}