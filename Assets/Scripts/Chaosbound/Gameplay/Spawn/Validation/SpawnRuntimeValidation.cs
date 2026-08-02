using Chaosbound.Debugging;
using Chaosbound.Gameplay.Spawn.Execution;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Validation
{
    /// <summary>
    /// Executes the Spawn Runtime validation pipeline.
    /// </summary>
    public static class SpawnRuntimeValidation
    {
        public static void Run(
            EnemyVariantData enemy)
        {
            if (enemy == null)
                throw new ArgumentNullException(nameof(enemy));

            SpawnRuntimeLogger.Step(
                "========================================");

            SpawnRuntimeLogger.Step(
                "Spawn Runtime Validation Started");

            SpawnRuntimeValidationBuilder builder =
                new SpawnRuntimeValidationBuilder()
                    .WithEnemy(enemy);

            SpawnRuntimeValidationContext context =
                builder.Build();

            SpawnRuntimeLogger.Success(
                "Validation context created.");

            SpawnRuntimeValidationBootstrap bootstrap =
                new SpawnRuntimeValidationBootstrap();

            SpawnJobExecutor executor =
                bootstrap.Build();

            SpawnRuntimeLogger.Success(
                "Runtime graph assembled.");

            SpawnRuntimeLogger.Step(
                "Executing Spawn Runtime...");

            executor.Execute(
                context.SchedulingContext);

            SpawnRuntimeLogger.Success(
                "Spawn Runtime Validation Completed.");

            SpawnRuntimeLogger.Step(
                "========================================");
        }
    }
}