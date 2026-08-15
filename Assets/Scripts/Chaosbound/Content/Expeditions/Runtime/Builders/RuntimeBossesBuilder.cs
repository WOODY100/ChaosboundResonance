using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Content.Expeditions.Definitions.Bosses;
using Chaosbound.Content.Expeditions.Runtime.Bosses;
using Chaosbound.Shared.Content.Entries;
using Chaosbound.Shared.Content.Resolution;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Builders
{
    /// <summary>
    /// Builds the runtime boss configuration from
    /// declarative expedition content.
    /// </summary>
    public sealed class RuntimeBossesBuilder
    {
        private readonly IContentResolver contentResolver;

        public RuntimeBossesBuilder(
            IContentResolver contentResolver)
        {
            this.contentResolver =
                contentResolver
                ?? throw new ArgumentNullException(
                    nameof(contentResolver));
        }

        public RuntimeBossesConfig BuildBosses(
            BossesDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(
                    nameof(definition));

            List<BossData> resolvedBosses =
                ResolveBosses(
                    definition.Entries);

            return new RuntimeBossesConfig(
                resolvedBosses);
        }

        private List<BossData> ResolveBosses(
            IReadOnlyList<ContentEntry> entries)
        {
            List<BossData> resolvedBosses =
                new(entries.Count);

            foreach (ContentEntry entry in entries)
            {
                BossData boss =
                    contentResolver.Resolve<BossData>(
                        entry.Id);

                resolvedBosses.Add(boss);
            }

            return resolvedBosses;
        }
    }
}