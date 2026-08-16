using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Bosses.Models;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Factories;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Bosses.Integration.Spawn
{
    /// <summary>
    /// Translates a BossSpawnPlan into a SpawnRequest.
    ///
    /// This class belongs to the integration boundary between
    /// the Boss Domain and the Spawn Domain.
    /// </summary>
    public sealed class BossSpawnRequestTranslator
    {
        private readonly SpawnRequestFactory
            spawnRequestFactory;

        private readonly SpawnRequestEntryFactory
            entryFactory;

        public BossSpawnRequestTranslator(
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
        /// Translates the supplied BossSpawnPlan into
        /// a SpawnRequest.
        /// </summary>
        public SpawnRequest Translate(
            BossSpawnPlan plan,
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
                BossSpawnPlanEntry planEntry
                in plan.Entries)
            {
                if (planEntry == null)
                {
                    throw new InvalidOperationException(
                        "BossSpawnPlan contains a null entry.");
                }

                entries.Add(
                    entryFactory.Create(
                        planEntry.Boss,
                        planEntry.Quantity));
            }

            return spawnRequestFactory.Create(
                entries,
                runtimeSpawnConfig,
                SpawnRequestOrigin.Boss);
        }
    }
}