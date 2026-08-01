using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Converts profile differences into tactical composition needs.
    /// </summary>
    public sealed class NeedsAnalyzer
    {
        /// <summary>
        /// Analyzes a profile comparison and produces the detected composition needs.
        /// </summary>
        /// <param name="comparison">
        /// The profile comparison to analyze.
        /// </param>
        /// <returns>
        /// A read-only collection of composition needs.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when comparison is null.
        /// </exception>
        public IReadOnlyList<CompositionNeed> Analyze(
            ProfileComparison comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException(nameof(comparison));
            }

            List<CompositionNeed> needs = new List<CompositionNeed>();

            foreach (CapabilityDifference difference in comparison.Differences)
            {
                if (difference.Difference == 0)
                {
                    continue;
                }

                CompositionNeed need = new CompositionNeed(
                    difference.Capability,
                    difference.Difference);

                needs.Add(need);
            }

            return needs;
        }
    }
}