using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Gameplay.Spawn.Execution;
using System;
using System.Collections.Generic;

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
            ISpawnMaterializer bossMaterializer,
            ISpawnMaterializer miniBossMaterializer)
        {
            if (enemyMaterializer == null)
                throw new ArgumentNullException(
                    nameof(enemyMaterializer));

            if (bossMaterializer == null)
                throw new ArgumentNullException(
                    nameof(bossMaterializer));

            if (miniBossMaterializer == null)
                throw new ArgumentNullException(
                    nameof(miniBossMaterializer));

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
            },
            {
                typeof(MiniBossData),
                miniBossMaterializer
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