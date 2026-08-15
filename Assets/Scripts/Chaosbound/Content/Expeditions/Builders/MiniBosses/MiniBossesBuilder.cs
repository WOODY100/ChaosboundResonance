using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Content.Expeditions.Authoring.MiniBosses;
using Chaosbound.Content.Expeditions.Definitions.MiniBosses;
using Chaosbound.Shared.Content.Entries;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.MiniBosses
{
    public static class MiniBossesBuilder
    {
        public static MiniBossesDefinition Build(
            MiniBossesAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<ContentEntry> content =
                BuildContent(authoring.Content);

            return new MiniBossesDefinition(
                content);
        }

        private static List<ContentEntry> BuildContent(
            IReadOnlyList<MiniBossData> authoring)
        {
            List<ContentEntry> result =
                new(authoring.Count);

            foreach (MiniBossData asset in authoring)
            {
                if (asset == null)
                {
                    throw new InvalidOperationException(
                        "MiniBossesAuthoring contains a null MiniBossData.");
                }

                result.Add(
                    new ContentEntry(
                        asset.Id,
                        asset));
            }

            return result;
        }
    }
}