using Chaosbound.Content.Expeditions.Definitions.Enemy;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Shared.Content.Entries;
using Chaosbound.Shared.Content.Resolution;
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
                ResolveEnemies(definition.Entries);

            return new RuntimeEnemyConfig(
                resolvedEnemies);
        }

        private List<EnemyVariantData> ResolveEnemies(
            IReadOnlyList<ContentEntry> entries)
        {
            List<EnemyVariantData> resolvedEnemies =
                new(entries.Count);

            foreach (ContentEntry entry in entries)
            {
                EnemyVariantData enemy =
                    contentResolver.Resolve<EnemyVariantData>(
                        entry.Id);

                resolvedEnemies.Add(enemy);
            }

            return resolvedEnemies;
        }
    }
}