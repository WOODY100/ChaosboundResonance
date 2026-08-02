using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Definitions;
using Chaosbound.Shared.Contracts;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnRequestEntry collections from gameplay spawn plans.
    /// </summary>
    public sealed class SpawnRequestEntryFactory
    {
        private readonly MaterializableReferenceFactory
            materializableReferenceFactory;

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

                IMaterializableReference reference =
                    materializableReferenceFactory.Create(
                        planEntry.Variant);

                MaterializableDefinition materializable =
                    new MaterializableDefinition(
                        reference);

                entries.Add(
                    new SpawnRequestEntry(
                        materializable,
                        planEntry.AllocatedQuantity));
            }

            return entries;
        }

        public SpawnRequestEntryFactory(
            MaterializableReferenceFactory materializableReferenceFactory)
        {
            this.materializableReferenceFactory =
                materializableReferenceFactory
                ?? throw new ArgumentNullException(
                    nameof(materializableReferenceFactory));
        }
    }
}