using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Services;
using System;

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

        private readonly SpawnExecutor
            spawnExecutor;

        public SpawnStage(
            SpawnRequestFactory requestFactory,
            SpawnExecutor spawnExecutor)
        {
            this.requestFactory =
                requestFactory
                ?? throw new ArgumentNullException(
                    nameof(requestFactory));

            this.spawnExecutor =
                spawnExecutor
                ?? throw new ArgumentNullException(
                    nameof(spawnExecutor));
        }

        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return true;
        }

        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            EnemySolverResult solverResult =
                context.State.EnemySolverResult;

            if (solverResult == null)
            {
                return;
            }

            SpawnPlan spawnPlan =
                solverResult.SpawnPlan;

            if (spawnPlan == null ||
                spawnPlan.IsEmpty)
            {
                return;
            }

            SpawnRequest request =
                requestFactory.Create(
                    spawnPlan,
                    context.Config.Spawn,
                    SpawnRequestOrigin.EnemySolver);

            // The Spawn Runtime execution will be connected
            // during the next implementation phase.
        }
    }
}