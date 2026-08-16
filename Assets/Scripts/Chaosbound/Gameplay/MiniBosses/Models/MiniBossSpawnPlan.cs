using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.MiniBosses.Models
{
    /// <summary>
    /// Represents the concrete materialization plan produced
    /// by the MiniBoss Domain.
    ///
    /// This plan describes which MiniBosses should be
    /// materialized and how many of each are required.
    ///
    /// It does not contain spawn positions, spawn points,
    /// placement information, or Spawn Runtime behavior.
    /// </summary>
    public sealed class MiniBossSpawnPlan
    {
        private readonly List<MiniBossSpawnPlanEntry>
            entries;

        /// <summary>
        /// Gets the concrete materialization entries.
        /// </summary>
        public IReadOnlyList<MiniBossSpawnPlanEntry>
            Entries =>
            entries;

        /// <summary>
        /// Gets whether this plan contains no entries.
        /// </summary>
        public bool IsEmpty =>
            entries.Count == 0;

        /// <summary>
        /// Gets the total number of MiniBosses requested
        /// by this plan.
        /// </summary>
        public int TotalQuantity
        {
            get
            {
                int total = 0;

                foreach (
                    MiniBossSpawnPlanEntry entry
                    in entries)
                {
                    total += entry.Quantity;
                }

                return total;
            }
        }

        public MiniBossSpawnPlan(
            IReadOnlyList<MiniBossSpawnPlanEntry> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(
                    nameof(entries));
            }

            this.entries =
                new List<MiniBossSpawnPlanEntry>(
                    entries.Count);

            foreach (
                MiniBossSpawnPlanEntry entry
                in entries)
            {
                if (entry == null)
                {
                    throw new InvalidOperationException(
                        "MiniBossSpawnPlan contains a null entry.");
                }

                this.entries.Add(entry);
            }
        }
    }
}