using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Content.Expeditions.Definitions.MiniBosses;
using Chaosbound.Content.Expeditions.Runtime.MiniBosses;
using Chaosbound.Shared.Content.Entries;
using Chaosbound.Shared.Content.Resolution;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Builders
{
    /// <summary>
    /// Builds the runtime mini boss configuration from
    /// declarative expedition content.
    /// </summary>
    public sealed class RuntimeMiniBossesBuilder
    {
        private readonly IContentResolver contentResolver;

        public RuntimeMiniBossesBuilder(
            IContentResolver contentResolver)
        {
            this.contentResolver =
                contentResolver
                ?? throw new ArgumentNullException(
                    nameof(contentResolver));
        }

        public RuntimeMiniBossesConfig BuildMiniBosses(
            MiniBossesDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(
                    nameof(definition));

            List<MiniBossData> resolvedMiniBosses =
                ResolveMiniBosses(
                    definition.Entries);

            return new RuntimeMiniBossesConfig(
                resolvedMiniBosses);
        }

        private List<MiniBossData> ResolveMiniBosses(
            IReadOnlyList<ContentEntry> entries)
        {
            List<MiniBossData> resolvedMiniBosses =
                new(entries.Count);

            foreach (ContentEntry entry in entries)
            {
                MiniBossData miniBoss =
                    contentResolver.Resolve<MiniBossData>(
                        entry.Id);

                resolvedMiniBosses.Add(miniBoss);
            }

            return resolvedMiniBosses;
        }
    }
}