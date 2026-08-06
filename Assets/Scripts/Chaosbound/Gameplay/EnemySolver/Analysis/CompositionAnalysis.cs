using Chaosbound.Gameplay.EnemySolver.Analysis.Models;
using Chaosbound.Gameplay.EnemySolver.Analysis.ValueObjects;
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
        /// Gets the current runtime tactical profile.
        /// </summary>
        public TacticalProfile CurrentProfile { get; }

        /// <summary>
        /// Gets the desired tactical profile.
        /// </summary>
        public TacticalProfile DesiredProfile { get; }

        /// <summary>
        /// Gets the comparison between the current and desired profiles.
        /// </summary>
        public ProfileComparison Comparison { get; }

        /// <summary>
        /// Gets the detected tactical needs.
        /// </summary>
        public IReadOnlyList<CompositionNeed> Needs { get; }

        /// <summary>
        /// Gets the currently selected tactical objective.
        /// </summary>
        public TacticalObjective Objective { get; }

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

        public CompositionAnalysis(
            TacticalProfile currentProfile,
            TacticalProfile desiredProfile,
            ProfileComparison comparison,
            IReadOnlyList<CompositionNeed> needs,
            TacticalObjective objective,
            IEnumerable<TacticalCapabilityCount> capabilityCounts,
            int totalAliveEnemies)
            : this(
                capabilityCounts,
                totalAliveEnemies)
        {
            CurrentProfile = currentProfile;

            DesiredProfile = desiredProfile;

            Comparison = comparison;

            Needs = needs
                ?? Array.Empty<CompositionNeed>();

            Objective = objective;
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
        /// Gets how many additional alive enemies are desired for the
        /// specified tactical capability.
        /// Returns zero when the current composition already satisfies
        /// or exceeds the desired profile.
        /// </summary>
        public int GetCapabilityDeficit(
            TacticalCapability capability)
        {
            return Math.Max(
                0,
                DesiredProfile.GetValue(capability)
                - CurrentProfile.GetValue(capability));
        }

        /// <summary>
        /// Gets how many alive enemies exceed the desired amount for the
        /// specified tactical capability.
        /// Returns zero when the capability is below or at the desired level.
        /// </summary>        
        public int GetCapabilityExcess(
            TacticalCapability capability)
        {
            return Math.Max(
                0,
                CurrentProfile.GetValue(capability)
                - DesiredProfile.GetValue(capability));
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
            return GetCapabilityDeficit(capability) > 0;
        }

        /// <summary>
        /// Gets whether the analyzed composition is empty.
        /// </summary>
        public bool IsEmpty =>
            TotalAliveEnemies == 0;
    }
}