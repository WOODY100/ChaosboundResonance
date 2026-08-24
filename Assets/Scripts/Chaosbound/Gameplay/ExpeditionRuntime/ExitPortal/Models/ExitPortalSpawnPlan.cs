using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Models
{
    /// <summary>
    /// Represents the declarative materialization plan
    /// produced by the Exit Portal Domain.
    /// </summary>
    public sealed class ExitPortalSpawnPlan
    {
        private readonly List<ExitPortalSpawnPlanEntry>
            entries;

        /// <summary>
        /// Gets the Exit Portal materialization entries.
        /// </summary>
        public IReadOnlyList<ExitPortalSpawnPlanEntry>
            Entries =>
            entries;

        /// <summary>
        /// Gets whether this plan contains no entries.
        /// </summary>
        public bool IsEmpty =>
            entries.Count == 0;

        /// <summary>
        /// Gets the total number of Exit Portal entities
        /// requested by this plan.
        /// </summary>
        public int TotalQuantity
        {
            get
            {
                int total = 0;

                foreach (
                    ExitPortalSpawnPlanEntry entry
                    in entries)
                {
                    total += entry.Quantity;
                }

                return total;
            }
        }

        public ExitPortalSpawnPlan(
            IReadOnlyList<ExitPortalSpawnPlanEntry> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(
                    nameof(entries));
            }

            this.entries =
                new List<ExitPortalSpawnPlanEntry>(
                    entries.Count);

            foreach (
                ExitPortalSpawnPlanEntry entry
                in entries)
            {
                if (entry == null)
                {
                    throw new InvalidOperationException(
                        "ExitPortalSpawnPlan contains a null entry.");
                }

                this.entries.Add(entry);
            }
        }
    }
}