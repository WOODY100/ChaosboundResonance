using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Models;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Factories;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Integration.Spawn
{
    /// <summary>
    /// Translates an ExitPortalSpawnPlan into a SpawnRequest.
    ///
    /// This class belongs to the integration boundary between
    /// the Exit Portal Domain and the Spawn Domain.
    /// </summary>
    public sealed class ExitPortalSpawnRequestTranslator
    {
        private readonly SpawnRequestFactory
            spawnRequestFactory;

        private readonly SpawnRequestEntryFactory
            entryFactory;

        public ExitPortalSpawnRequestTranslator(
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
        /// Translates the supplied ExitPortalSpawnPlan
        /// into a SpawnRequest.
        /// </summary>
        public SpawnRequest Translate(
            ExitPortalSpawnPlan plan,
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
                ExitPortalSpawnPlanEntry planEntry
                in plan.Entries)
            {
                if (planEntry == null)
                {
                    throw new InvalidOperationException(
                        "ExitPortalSpawnPlan contains a null entry.");
                }

                entries.Add(
                    entryFactory.Create(
                        planEntry.ExitPortal,
                        planEntry.Quantity));
            }

            return spawnRequestFactory.Create(
                entries,
                runtimeSpawnConfig,
                SpawnRequestOrigin.ExitPortal);
        }
    }
}