using Chaosbound.Content.Expeditions.Definitions.Enemy;
using Chaosbound.Content.Expeditions.Definitions.Enemy.TacticalIdentity;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Content.Expeditions.Runtime.Enemy.TacticalIdentity;
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

            RuntimeTacticalIdentity tacticalIdentity =
                BuildRuntimeTacticalIdentity(
                    definition.TacticalIdentity);

            return new RuntimeEnemyConfig(
                resolvedEnemies,
                definition.SchedulingPolicy,
                tacticalIdentity);
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

        private static RuntimeTacticalIdentity BuildRuntimeTacticalIdentity(
            TacticalIdentityDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            List<RuntimeCapabilityAffinity> affinities =
                BuildRuntimeAffinities(
                    definition.Affinities);

            return new RuntimeTacticalIdentity(
                affinities);
        }

        private static List<RuntimeCapabilityAffinity> BuildRuntimeAffinities(
            IReadOnlyList<CapabilityAffinityDefinition> definitions)
        {
            List<RuntimeCapabilityAffinity> result =
                new(definitions.Count);

            foreach (CapabilityAffinityDefinition affinity in definitions)
            {
                if (affinity == null)
                {
                    throw new InvalidOperationException(
                        "TacticalIdentityDefinition contains a null CapabilityAffinityDefinition.");
                }

                result.Add(
                    new RuntimeCapabilityAffinity(
                        affinity.Capability,
                        affinity.BonusScore));
            }

            return result;
        }
    }
}