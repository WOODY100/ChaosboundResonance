using Chaosbound.Content.Expeditions.Runtime.Enemy.TacticalIdentity;
using Chaosbound.Gameplay.EnemySolver.Analysis.Models;
using Chaosbound.Gameplay.EnemySolver.Analysis.Runtime;
using Chaosbound.Gameplay.EnemySolver.Analysis.Services;
using Chaosbound.Gameplay.EnemySolver.Analysis.ValueObjects;
using Chaosbound.Gameplay.EnemySolver.Enums;
using Chaosbound.Gameplay.EnemySolver.Runtime.Composition;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.EnemySolver.Analysis
{
    /// <summary>
    /// Produces a tactical analysis from the current runtime composition.
    /// </summary>
    public sealed class CompositionAnalyzer
    {
        private readonly RuntimeTacticalProfileBuilder
            runtimeProfileBuilder;

        private readonly ProfileComparator
            profileComparator;

        private readonly NeedsAnalyzer
            needsAnalyzer;

        private readonly ObjectiveSelector
            objectiveSelector;



        /// <summary>
        /// Analyzes the current runtime composition.
        /// </summary>
        public CompositionAnalysis Analyze(
            RuntimeCompositionState runtimeComposition,
            RuntimeTacticalIdentity tacticalIdentity)
        {
            if (runtimeComposition == null)
                throw new ArgumentNullException(nameof(runtimeComposition));

            if (tacticalIdentity == null)
            {
                throw new ArgumentNullException(
                    nameof(tacticalIdentity));
            }

            Dictionary<TacticalCapability, int> counts =
                new();

            int totalAliveEnemies = 0;

            foreach (RuntimeCompositionEntry entry in runtimeComposition.Entries)
            {
                totalAliveEnemies += entry.AliveCount;

                foreach (TacticalCapability capability
                    in entry.Variant.TacticalCapabilities)
                {
                    if (counts.TryGetValue(
                        capability,
                        out int current))
                    {
                        counts[capability] =
                            current + entry.AliveCount;
                    }
                    else
                    {
                        counts.Add(
                            capability,
                            entry.AliveCount);
                    }
                }
            }

            List<TacticalCapabilityCount> capabilityCounts =
                new(counts.Count);

            foreach (KeyValuePair<TacticalCapability, int> pair in counts)
            {
                capabilityCounts.Add(
                    new TacticalCapabilityCount(
                        pair.Key,
                        pair.Value));
            }

            TacticalProfile currentProfile =
                runtimeProfileBuilder.Build(
                    runtimeComposition);

            TacticalProfile desiredProfile =
                BuildDesiredProfile(tacticalIdentity);

            ProfileComparison comparison =
                profileComparator.Compare(
                    currentProfile,
                    desiredProfile);

            IReadOnlyList<CompositionNeed> needs =
                needsAnalyzer.Analyze(
                    comparison);

            TacticalObjective objective =
                needs.Count > 0
                    ? objectiveSelector.Select(needs)
                    : null;

            return new CompositionAnalysis(
                currentProfile,
                desiredProfile,
                comparison,
                needs,
                objective,
                capabilityCounts,
                totalAliveEnemies);
        }

        private TacticalProfile BuildDesiredProfile(
            RuntimeTacticalIdentity tacticalIdentity)
        {
            TacticalProfile desiredProfile = new TacticalProfile();

            foreach (RuntimeCapabilityAffinity affinity
                in tacticalIdentity.Affinities)
            {
                desiredProfile.SetValue(
                    affinity.Capability,
                    affinity.DesiredCount);
            }

            return desiredProfile;
        }

        public CompositionAnalyzer(
            RuntimeTacticalProfileBuilder runtimeProfileBuilder,
            ProfileComparator profileComparator,
            NeedsAnalyzer needsAnalyzer,
            ObjectiveSelector objectiveSelector)
        {
            this.runtimeProfileBuilder =
                runtimeProfileBuilder
                ?? throw new ArgumentNullException(
                    nameof(runtimeProfileBuilder));

            this.profileComparator =
                profileComparator
                ?? throw new ArgumentNullException(
                    nameof(profileComparator));

            this.needsAnalyzer =
                needsAnalyzer
                ?? throw new ArgumentNullException(
                    nameof(needsAnalyzer));

            this.objectiveSelector =
                objectiveSelector
                ?? throw new ArgumentNullException(
                    nameof(objectiveSelector));
        }
    }
}