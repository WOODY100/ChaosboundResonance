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
        /// <summary>
        /// Analyzes the current runtime composition.
        /// </summary>
        public CompositionAnalysis Analyze(
            RuntimeCompositionState runtimeComposition)
        {
            if (runtimeComposition == null)
                throw new ArgumentNullException(nameof(runtimeComposition));

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

            return new CompositionAnalysis(
                capabilityCounts,
                totalAliveEnemies);
        }
    }
}