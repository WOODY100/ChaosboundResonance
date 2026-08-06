using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Stages
{
    /// <summary>
    /// Executes the Spawn Runtime for the current expedition tick.
    /// </summary>
    public sealed class SpawnStage :
        IExpeditionRuntimeStage
    {
        private readonly SpawnRequestFactory
            requestFactory;

        private readonly SpawnRuntime
            spawnRuntime;

        public SpawnStage(
            SpawnRequestFactory requestFactory,
            SpawnRuntime spawnRuntime)
        {
            this.requestFactory =
                requestFactory
                ?? throw new ArgumentNullException(
                    nameof(requestFactory));

            this.spawnRuntime =
                spawnRuntime
                ?? throw new ArgumentNullException(
                    nameof(spawnRuntime));
        }

        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            EnemySolverResult result =
                context.State.EnemySolverResult;

            Debug.Log(
                $"[SpawnStage] Result={result != null} | " +
                $"Empty={result?.SpawnPlan.IsEmpty}");

            return result != null
                && !result.SpawnPlan.IsEmpty;
        }

        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            EnemySolverResult solverResult =
                context.State.EnemySolverResult;

            if (solverResult == null)
            {
                throw new InvalidOperationException(
                    "SpawnStage requires an EnemySolverResult.");
            }

            SpawnRequest request =
                requestFactory.Create(
                    solverResult.SpawnPlan,
                    context.Config.Spawn,
                    SpawnRequestOrigin.EnemySolver);

            Debug.Log(
                $"[SpawnStage] SpawnRequest Entries={request.Entries.Count}");

            spawnRuntime.Execute(
                request,
                context.Config.Enemy,
                context.Config.Spawn,
                context.References.Runtime,
                context.State.PressureSnapshot,
                context.State);

            Debug.Log("[SpawnStage] SpawnRuntime.Execute finished.");
        }
    }
}