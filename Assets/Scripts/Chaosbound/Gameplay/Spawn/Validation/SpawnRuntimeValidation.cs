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

            SpawnRuntimeDebugger.Step(
                "========================================");

            SpawnRuntimeDebugger.Step(
                "Spawn Runtime Validation Started");

            SpawnRuntimeValidationBuilder builder =
                new SpawnRuntimeValidationBuilder()
                    .WithEnemy(enemy);

            SpawnRuntimeValidationContext context =
                builder.Build();

            /*SpawnRuntimeDebugger.Success(
                "Validation context created.");
            */
            SpawnRuntimeValidationBootstrap bootstrap =
                new SpawnRuntimeValidationBootstrap();

            SpawnJobExecutor executor =
                bootstrap.Build();

            /*SpawnRuntimeDebugger.Success(
                "Runtime graph assembled.");
                        */
            SpawnRuntimeDebugger.Step(
                "Executing Spawn Runtime...");

            executor.Execute(
                context.SchedulingContext);

            /*SpawnRuntimeDebugger.Success(
                "Spawn Runtime Validation Completed.");
            */
            SpawnRuntimeDebugger.Step(
                "========================================");
        }
    }
}