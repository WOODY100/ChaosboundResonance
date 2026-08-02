using Chaosbound.Content.Expeditions.Definitions.Enemy;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Shared.Content.Resolution;
using Chaosbound.Shared.Content.Entries;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Builders
{
    /// <summary>
    /// Builds the runtime enemy configuration from the declarative expedition content.
    /// </summary>
    public sealed class RuntimeEnemyBuilder
    {
        private readonly IContentResolver contentResolver;

        public RuntimeEnemyBuilder(
            IContentResolver contentResolver)
        {
            this.contentResolver = contentResolver
                ?? throw new ArgumentNullException(nameof(contentResolver));
        }

        public RuntimeEnemyConfig BuildEnemy(
            EnemyDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            List<EnemyVariantData> resolvedEnemies =
                new(definition.Entries.Count);

            foreach (ContentEntry entry in definition.Entries)
            {
                EnemyVariantData enemy =
                    contentResolver.Resolve<EnemyVariantData>(entry.Id);

                resolvedEnemies.Add(enemy);
            }

            return new RuntimeEnemyConfig(
                resolvedEnemies,
                definition.SchedulingPolicy);
        }
    }
}