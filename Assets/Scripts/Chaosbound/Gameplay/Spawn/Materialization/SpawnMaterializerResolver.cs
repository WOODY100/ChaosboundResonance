using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Content.Enemy.Bosses;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    /// <summary>
    /// Resolves the materializer responsible for
    /// materializing a SpawnTask.
    /// </summary>
    public sealed class SpawnMaterializerResolver
    {
        private readonly IReadOnlyDictionary<
            Type,
            ISpawnMaterializer> materializers;

        public SpawnMaterializerResolver(
            ISpawnMaterializer enemyMaterializer,
            ISpawnMaterializer bossMaterializer)
        {
            if (enemyMaterializer == null)
                throw new ArgumentNullException(nameof(enemyMaterializer));

            if (bossMaterializer == null)
                throw new ArgumentNullException(nameof(bossMaterializer));

            materializers =
                new Dictionary<Type, ISpawnMaterializer>
                {
                    {
                        typeof(EnemyVariantData),
                        enemyMaterializer
                    },
                    {
                        typeof(BossData),
                        bossMaterializer
                    }
                };
        }

        /// <summary>
        /// Resolves the materializer associated with the supplied execution context.
        /// </summary>
        public ISpawnMaterializer Resolve(
            SpawnExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Type referenceType =
                context
                    .ResolvedTask
                    .ScheduledTask
                    .Task
                    .Entry
                    .Materializable
                    .Reference
                    .GetType();

            if (!materializers.TryGetValue(
                referenceType,
                out ISpawnMaterializer materializer))
            {
                throw new InvalidOperationException(
                    $"No materializer registered for '{referenceType.Name}'.");
            }

            return materializer;
        }
    }
}