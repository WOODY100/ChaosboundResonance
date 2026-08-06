using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.EnemySolver.Runtime.Builders;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using System;
using UnityEngine;

using EnemySolverService =
    Chaosbound.Gameplay.EnemySolver.Services.EnemySolver;

namespace Chaosbound.Gameplay.EnemySolver.Runtime.Stages
{
    /// <summary>
    /// Evaluates the desired enemy composition for the current
    /// expedition state.
    /// </summary>
    public sealed class EnemyCompositionStage :
        IExpeditionRuntimeStage
    {
        private readonly EnemySolverRequestBuilder
            requestBuilder;

        private readonly EnemySolverService
            enemySolver;

        /// <summary>
        /// Creates a new Enemy Composition Stage.
        /// </summary>
        public EnemyCompositionStage(
            EnemySolverRequestBuilder requestBuilder,
            EnemySolverService enemySolver)
        {
            this.requestBuilder =
                requestBuilder
                ?? throw new ArgumentNullException(
                    nameof(requestBuilder));

            this.enemySolver =
                enemySolver
                ?? throw new ArgumentNullException(
                    nameof(enemySolver));
        }

        /// <inheritdoc/>
        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            return true;
        }

        /// <inheritdoc/>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            EnemySolverRequest request =
                requestBuilder.Build(
                    context);

            EnemySolverResult result =
                enemySolver.Solve(
                    request);

            Debug.Log(
                $"[EnemySolver] Composition={result.Composition.Entries.Count} | " +
                $"SpawnEntries={result.SpawnPlan.Entries.Count} | " +
                $"Allocated={result.SpawnPlan.TotalAllocatedEnemyCount}");

            context.State.SetEnemySolverResult(
                result);
        }
    }
}