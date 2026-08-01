using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Threat.ValueObjects;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Allocates the available threat capacity across the desired
    /// enemy composition to produce the executable spawn plan.
    /// </summary>
    public sealed class BudgetAllocator
    {
        /// <summary>
        /// Produces a spawn plan from the specified target composition.
        /// </summary>
        /// <param name="composition">
        /// Target enemy composition.
        /// </param>
        /// <param name="availableThreat">
        /// Available threat capacity.
        /// </param>
        /// <returns>
        /// Spawn plan constrained by the available threat capacity.
        /// </returns>
        public SpawnPlan Allocate(
            EnemyComposition composition,
            ThreatCapacity availableThreat)
        {
            if (composition == null)
                throw new ArgumentNullException(nameof(composition));

            float remainingThreat = availableThreat.Value;

            List<SpawnPlanEntry> entries = new();

            foreach (EnemyCompositionEntry compositionEntry in composition.Entries)
            {
                entries.Add(
                    AllocateEntry(
                        compositionEntry,
                        ref remainingThreat));
            }

            return new SpawnPlan(entries);
        }

        /// <summary>
        /// Allocates the available threat budget for a single composition entry.
        /// </summary>
        /// <param name="entry">
        /// Composition entry to allocate.
        /// </param>
        /// <param name="remainingThreat">
        /// Remaining threat budget.
        /// </param>
        /// <returns>
        /// Spawn plan entry representing the allocation result.
        /// </returns>
        private static SpawnPlanEntry AllocateEntry(
            EnemyCompositionEntry entry,
            ref float remainingThreat)
        {
            EnemyVariantData variant = entry.Variant;

            float threatCost = variant.ThreatCost.Value;

            int allocatedQuantity = 0;

            while (
                allocatedQuantity < entry.Quantity &&
                remainingThreat >= threatCost)
            {
                allocatedQuantity++;

                remainingThreat -= threatCost;
            }

            return new SpawnPlanEntry(
                variant,
                entry.Quantity,
                allocatedQuantity);
        }
    }
}