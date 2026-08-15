using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Content.Expeditions.Authoring.Bosses;
using Chaosbound.Content.Expeditions.Definitions.Bosses;
using Chaosbound.Shared.Content.Entries;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.Bosses
{
    public static class BossesBuilder
    {
        public static BossesDefinition Build(
            BossesAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<ContentEntry> content =
                BuildContent(authoring.Content);

            return new BossesDefinition(
                content);
        }

        private static List<ContentEntry> BuildContent(
            IReadOnlyList<BossData> authoring)
        {
            List<ContentEntry> result =
                new(authoring.Count);

            foreach (BossData asset in authoring)
            {
                if (asset == null)
                {
                    throw new InvalidOperationException(
                        "BossesAuthoring contains a null BossData.");
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