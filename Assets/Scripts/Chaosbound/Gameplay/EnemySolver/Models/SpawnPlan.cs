using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Chaosbound.Gameplay.Threat.ValueObjects;

namespace Chaosbound.Gameplay.EnemySolver.Models
{
    /// <summary>
    /// Represents the executable spawn plan generated after
    /// allocating the available threat budget.
    /// </summary>
    public sealed class SpawnPlan
    {
        private readonly IReadOnlyList<SpawnPlanEntry> m_Entries;

        public SpawnPlan(
            IEnumerable<SpawnPlanEntry> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            List<SpawnPlanEntry> list = entries.ToList();

            m_Entries = new ReadOnlyCollection<SpawnPlanEntry>(list);
        }

        /// <summary>
        /// Gets the immutable spawn plan entries.
        /// </summary>
        public IReadOnlyList<SpawnPlanEntry> Entries => m_Entries;

        /// <summary>
        /// Gets whether the spawn plan contains no entries.
        /// </summary>
        public bool IsEmpty => m_Entries.Count == 0;

        /// <summary>
        /// Gets the total desired enemy count.
        /// </summary>
        public int TotalDesiredEnemyCount =>
            m_Entries.Sum(entry => entry.DesiredQuantity);

        /// <summary>
        /// Gets the total allocated enemy count.
        /// </summary>
        public int TotalAllocatedEnemyCount =>
            m_Entries.Sum(entry => entry.AllocatedQuantity);

        /// <summary>
        /// Gets the total pending enemy count.
        /// </summary>
        public int TotalPendingEnemyCount =>
            m_Entries.Sum(entry => entry.PendingQuantity);

        /// <summary>
        /// Gets the total allocated threat cost.
        /// </summary>
        public ThreatCost TotalAllocatedThreatCost =>
            new ThreatCost(
                m_Entries.Sum(entry =>
                    entry.Variant.ThreatCost.Value *
                    entry.AllocatedQuantity));
    }
}