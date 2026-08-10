using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Factories;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Integration.Spawn
{
    /// <summary>
    /// Translates a CombatSpawnPlan into a SpawnRequest.
    ///
    /// This class belongs to the integration boundary between
    /// the Combat domain and the Spawn domain.
    /// </summary>
    public sealed class CombatSpawnRequestTranslator
    {
        private readonly SpawnRequestFactory
            spawnRequestFactory;

        private readonly SpawnRequestEntryFactory
            entryFactory;

        public CombatSpawnRequestTranslator(
            SpawnRequestFactory spawnRequestFactory,
            SpawnRequestEntryFactory entryFactory)
        {
            this.spawnRequestFactory =
                spawnRequestFactory
                ?? throw new ArgumentNullException(
                    nameof(spawnRequestFactory));

            this.entryFactory =
                entryFactory
                ?? throw new ArgumentNullException(
                    nameof(entryFactory));
        }

        /// <summary>
        /// Translates the supplied CombatSpawnPlan into
        /// a SpawnRequest.
        /// </summary>
        public SpawnRequest Translate(
            CombatSpawnPlan plan,
            RuntimeSpawnConfig runtimeSpawnConfig)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(
                    nameof(plan));
            }

            if (runtimeSpawnConfig == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeSpawnConfig));
            }

            List<SpawnRequestEntry> entries =
                new List<SpawnRequestEntry>(
                    plan.Entries.Count);

            foreach (
                CombatSpawnPlanEntry planEntry
                in plan.Entries)
            {
                if (planEntry == null)
                {
                    throw new InvalidOperationException(
                        "CombatSpawnPlan contains a null entry.");
                }

                entries.Add(
                    entryFactory.Create(
                        planEntry.Variant,
                        planEntry.Quantity));
            }

            return spawnRequestFactory.Create(
                entries,
                runtimeSpawnConfig,
                SpawnRequestOrigin.Combat);
        }
    }
}