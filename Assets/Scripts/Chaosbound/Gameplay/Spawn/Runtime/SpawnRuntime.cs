using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        private readonly List<GameObject>
            materializedObjects =
                new List<GameObject>();

        public IReadOnlyList<GameObject> MaterializedObjects =>
            materializedObjects;

        public int MaterializedObjectCount =>
            materializedObjects.Count;

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
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            ExpeditionRuntimeState expeditionRuntime)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (spawnConfig == null)
                throw new ArgumentNullException(nameof(spawnConfig));

            if (references == null)
                throw new ArgumentNullException(nameof(references));

            if (expeditionRuntime == null)
                throw new ArgumentNullException(nameof(expeditionRuntime));

            SpawnExecutionPlan executionPlan =
                spawnExecutor.Execute(
                    request);

            IReadOnlyList<GameObject> spawnedObjects =
                executionPlanExecutor.Execute(
                    executionPlan,
                    spawnConfig,
                    references,
                    expeditionRuntime);

            materializedObjects.AddRange(
                spawnedObjects);
        }

        public void Cleanup()
        {
            foreach (GameObject obj in materializedObjects)
            {
                if (obj == null)
                    continue;

                PooledObject pooledObject =
                    obj.GetComponent<PooledObject>();

                if (pooledObject != null)
                {
                    pooledObject.ReturnToPool();
                }
            }

            materializedObjects.Clear();
        }
    }
}