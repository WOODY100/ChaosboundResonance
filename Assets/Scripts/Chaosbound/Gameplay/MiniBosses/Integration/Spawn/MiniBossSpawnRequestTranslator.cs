using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.MiniBosses.Models;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Factories;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.MiniBosses.Integration.Spawn
{
    /// <summary>
    /// Translates a MiniBossSpawnPlan into a SpawnRequest.
    ///
    /// This class belongs to the integration boundary between
    /// the MiniBoss Domain and the Spawn Domain.
    /// </summary>
    public sealed class MiniBossSpawnRequestTranslator
    {
        private readonly SpawnRequestFactory
            spawnRequestFactory;

        private readonly SpawnRequestEntryFactory
            entryFactory;

        public MiniBossSpawnRequestTranslator(
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
        /// Translates the supplied MiniBossSpawnPlan
        /// into a SpawnRequest.
        /// </summary>
        public SpawnRequest Translate(
            MiniBossSpawnPlan plan,
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
                MiniBossSpawnPlanEntry planEntry
                in plan.Entries)
            {
                if (planEntry == null)
                {
                    throw new InvalidOperationException(
                        "MiniBossSpawnPlan contains a null entry.");
                }

                entries.Add(
                    entryFactory.Create(
                        planEntry.MiniBoss,
                        planEntry.Quantity));
            }

            return spawnRequestFactory.Create(
                entries,
                runtimeSpawnConfig,
                SpawnRequestOrigin.MiniBoss);
        }
    }
}