using System;
using UnityEngine;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Services;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Pressure.Models;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;

namespace Chaosbound.Gameplay.Spawn.Runtime
{
    /// <summary>
    /// Represents the public entry point of the Spawn Runtime.
    /// Coordinates the complete execution pipeline required
    /// to materialize gameplay entities.
    /// </summary>
    public sealed class SpawnRuntime
    {
        private readonly SpawnExecutor
            spawnExecutor;

        private readonly SpawnExecutionPlanExecutor
            executionPlanExecutor;

        /// <summary>
        /// Creates a new SpawnRuntime.
        /// </summary>
        public SpawnRuntime(
            SpawnExecutor spawnExecutor,
            SpawnExecutionPlanExecutor executionPlanExecutor)
        {
            this.spawnExecutor =
                spawnExecutor
                ?? throw new ArgumentNullException(
                    nameof(spawnExecutor));

            this.executionPlanExecutor =
                executionPlanExecutor
                ?? throw new ArgumentNullException(
                    nameof(executionPlanExecutor));
        }

        /// <summary>
        /// Executes a declarative SpawnRequest.
        /// </summary>
        public void Execute(
            SpawnRequest request,
            RuntimeEnemyConfig enemyConfig,
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            PressureSnapshot pressure,
            ExpeditionRuntimeState expeditionRuntime)
        {
            Debug.Log(
                $"[SpawnRuntime] Request Entries={request.Entries.Count}");

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (enemyConfig == null)
                throw new ArgumentNullException(nameof(enemyConfig));

            if (spawnConfig == null)
                throw new ArgumentNullException(nameof(spawnConfig));

            if (references == null)
                throw new ArgumentNullException(nameof(references));

            if (pressure == null)
                throw new ArgumentNullException(nameof(pressure));

            if (expeditionRuntime == null)
                throw new ArgumentNullException(nameof(expeditionRuntime));

            SpawnExecutionPlan executionPlan =
                spawnExecutor.Execute(
                    request);

            Debug.Log(
                $"[SpawnRuntime] ExecutionPlan Entries={executionPlan.Entries.Count}");

            executionPlanExecutor.Execute(
                executionPlan,
                enemyConfig,
                spawnConfig,
                references,
                pressure,
                expeditionRuntime);
        }
    }
}