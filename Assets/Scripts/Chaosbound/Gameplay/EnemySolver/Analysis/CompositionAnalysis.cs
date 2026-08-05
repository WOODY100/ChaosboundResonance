using Chaosbound.Gameplay.EnemySolver.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chaosbound.Gameplay.EnemySolver.Analysis
{
    /// <summary>
    /// Represents the tactical analysis of the current runtime composition.
    /// </summary>
    public sealed class CompositionAnalysis
    {
        private readonly IReadOnlyList<TacticalCapabilityCount> capabilityCounts;

        private readonly Dictionary<TacticalCapability, int> capabilityLookup;

        /// <summary>
        /// Gets the tactical capability counts.
        /// </summary>
        public IReadOnlyList<TacticalCapabilityCount> CapabilityCounts =>
            capabilityCounts;

        /// <summary>
        /// Gets the total number of alive enemies.
        /// </summary>
        public int TotalAliveEnemies { get; }

        /// <summary>
        /// Creates a new composition analysis.
        /// </summary>
        public CompositionAnalysis(
            IEnumerable<TacticalCapabilityCount> capabilityCounts,
            int totalAliveEnemies)
        {
            if (capabilityCounts == null)
                throw new ArgumentNullException(nameof(capabilityCounts));

            if (totalAliveEnemies < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(totalAliveEnemies),
                    totalAliveEnemies,
                    "Alive enemy count cannot be negative.");
            }

            this.capabilityCounts =
                new ReadOnlyCollection<TacticalCapabilityCount>(
                    capabilityCounts.ToList());

            capabilityLookup =
                new Dictionary<TacticalCapability, int>(
                    this.capabilityCounts.Count);

            foreach (TacticalCapabilityCount capability in this.capabilityCounts)
            {
                capabilityLookup[capability.Capability] =
                    capability.Count;
            }

            TotalAliveEnemies = totalAliveEnemies;
        }

        /// <summary>
        /// Gets how many alive enemies provide the specified tactical capability.
        /// Returns zero when the capability is not present.
        /// </summary>
        public int GetCapabilityCount(
            TacticalCapability capability)
        {
            return capabilityLookup.TryGetValue(
                capability,
                out int count)
                ? count
                : 0;
        }

        /// <summary>
        /// Returns whether at least one alive enemy currently provides
        /// the specified tactical capability.
        /// </summary>
        public bool HasCapability(
            TacticalCapability capability)
        {
            return GetCapabilityCount(capability) > 0;
        }

        /// <summary>
        /// Returns whether no alive enemy currently provides
        /// the specified tactical capability.
        /// </summary>
        public bool NeedsCapability(
            TacticalCapability capability)
        {
            return !HasCapability(capability);
        }

        /// <summary>
        /// Gets whether the analyzed composition is empty.
        /// </summary>
        public bool IsEmpty =>
            TotalAliveEnemies == 0;
    }
}