using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Selects the tactical objective that should be addressed next.
    /// </summary>
    public sealed class ObjectiveSelector
    {
        /// <summary>
        /// Selects the highest-priority tactical objective from the detected needs.
        /// </summary>
        /// <param name="needs">
        /// The detected composition needs.
        /// </param>
        /// <returns>
        /// The selected tactical objective.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the needs collection is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no tactical needs are available.
        /// </exception>
        public TacticalObjective Select(
            IReadOnlyList<CompositionNeed> needs)
        {
            if (needs == null)
            {
                throw new ArgumentNullException(nameof(needs));
            }

            if (needs.Count == 0)
            {
                throw new InvalidOperationException(
                    "No tactical needs are available to select an objective.");
            }

            CompositionNeed bestDeficit = null;
            CompositionNeed bestExcess = null;

            for (int i = 0; i < needs.Count; i++)
            {
                CompositionNeed need = needs[i];

                if (need.IsDeficit)
                {
                    if (bestDeficit == null ||
                        need.Difference > bestDeficit.Difference)
                    {
                        bestDeficit = need;
                    }

                    continue;
                }

                if (bestExcess == null ||
                    need.Difference < bestExcess.Difference)
                {
                    bestExcess = need;
                }
            }

            if (bestDeficit != null)
            {
                return new TacticalObjective(
                    bestDeficit.Capability,
                    bestDeficit.Difference);
            }

            return new TacticalObjective(
                bestExcess.Capability,
                bestExcess.Difference);
        }
    }
}