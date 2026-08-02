using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Definitions;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnRequestEntry collections from gameplay spawn plans.
    /// </summary>
    public sealed class SpawnRequestEntryFactory
    {
        /// <summary>
        /// Creates the SpawnRequest entries represented by the specified SpawnPlan.
        /// </summary>
        public IReadOnlyList<SpawnRequestEntry> Create(
            SpawnPlan spawnPlan)
        {
            if (spawnPlan == null)
            {
                throw new ArgumentNullException(nameof(spawnPlan));
            }

            List<SpawnRequestEntry> entries =
                new List<SpawnRequestEntry>();

            foreach (SpawnPlanEntry planEntry in spawnPlan.Entries)
            {
                if (planEntry.AllocatedQuantity <= 0)
                {
                    continue;
                }

                MaterializableDefinition materializable =
                    new MaterializableDefinition(
                        planEntry.Variant);

                entries.Add(
                    new SpawnRequestEntry(
                        materializable,
                        planEntry.AllocatedQuantity));
            }

            return entries;
        }
    }
}