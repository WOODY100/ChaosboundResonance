using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Execution
{
    /// <summary>
    /// Executes SpawnExecutionPlans by coordinating the
    /// execution of every SpawnJob contained in the plan.
    /// </summary>
    public sealed class SpawnExecutionPlanExecutor
    {
        private readonly SpawnJobFactory
            spawnJobFactory;

        private readonly EnemySchedulingContextFactory
            schedulingContextFactory;

        private readonly SpawnJobExecutor
            spawnJobExecutor;

        /// <summary>
        /// Creates a new SpawnExecutionPlanExecutor.
        /// </summary>
        public SpawnExecutionPlanExecutor(
            SpawnJobFactory spawnJobFactory,
            EnemySchedulingContextFactory schedulingContextFactory,
            SpawnJobExecutor spawnJobExecutor)
        {
            this.spawnJobFactory =
                spawnJobFactory
                ?? throw new ArgumentNullException(
                    nameof(spawnJobFactory));

            this.schedulingContextFactory =
                schedulingContextFactory
                ?? throw new ArgumentNullException(
                    nameof(schedulingContextFactory));

            this.spawnJobExecutor =
                spawnJobExecutor
                ?? throw new ArgumentNullException(
                    nameof(spawnJobExecutor));
        }

        /// <summary>
        /// Executes the supplied SpawnExecutionPlan.
        /// </summary>
        public void Execute(
    SpawnExecutionPlan executionPlan,
    RuntimeEnemyConfig enemyConfig,
    RuntimeSpawnConfig spawnConfig,
    RuntimeReferencesConfig references,
    ExpeditionRuntimeState expeditionRuntime)
        {
            if (executionPlan == null)
                throw new ArgumentNullException(nameof(executionPlan));

            if (enemyConfig == null)
                throw new ArgumentNullException(nameof(enemyConfig));

            if (spawnConfig == null)
                throw new ArgumentNullException(nameof(spawnConfig));

            if (references == null)
                throw new ArgumentNullException(nameof(references));

            if (expeditionRuntime == null)
                throw new ArgumentNullException(nameof(expeditionRuntime));

            IReadOnlyList<SpawnJob> jobs =
                spawnJobFactory.Create(
                    executionPlan);

            foreach (SpawnJob job in jobs)
            {
                EnemySchedulingContext schedulingContext =
                    schedulingContextFactory.Create(
                        job,
                        enemyConfig,
                        spawnConfig,
                        references,
                        expeditionRuntime);

                spawnJobExecutor.Execute(
                    schedulingContext);
            }
        }
    }
}