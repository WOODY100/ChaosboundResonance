using Chaosbound.Gameplay.Combat.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Services
{
    /// <summary>
    /// Builds a concrete CombatSpawnPlan from selected
    /// enemy variants.
    ///
    /// This builder does not select variants. It only groups
    /// already selected variants into materialization quantities.
    /// </summary>
    public sealed class CombatSpawnPlanBuilder
    {
        public CombatSpawnPlan Build(
            IReadOnlyList<EnemyVariantData> variants)
        {
            if (variants == null)
            {
                throw new ArgumentNullException(
                    nameof(variants));
            }

            Dictionary<EnemyVariantData, int> quantities =
                new Dictionary<EnemyVariantData, int>();

            foreach (
                EnemyVariantData variant
                in variants)
            {
                if (variant == null)
                {
                    throw new InvalidOperationException(
                        "Selected enemy variants cannot contain null entries.");
                }

                if (quantities.TryGetValue(
                    variant,
                    out int quantity))
                {
                    quantities[variant] =
                        quantity + 1;
                }
                else
                {
                    quantities.Add(
                        variant,
                        1);
                }
            }

            List<CombatSpawnPlanEntry> entries =
                new List<CombatSpawnPlanEntry>(
                    quantities.Count);

            foreach (
                KeyValuePair<EnemyVariantData, int> pair
                in quantities)
            {
                entries.Add(
                    new CombatSpawnPlanEntry(
                        pair.Key,
                        pair.Value));
            }

            return new CombatSpawnPlan(
                entries);
        }
    }
}