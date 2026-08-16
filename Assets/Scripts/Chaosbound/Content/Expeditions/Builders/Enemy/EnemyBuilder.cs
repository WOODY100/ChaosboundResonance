using Chaosbound.Content.Expeditions.Authoring.Enemy;
using Chaosbound.Content.Expeditions.Definitions.Enemy;
using Chaosbound.Shared.Content.Entries;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.Enemy
{
    public static class EnemyBuilder
    {
        public static EnemyDefinition Build(
            EnemyAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<ContentEntry> content =
                BuildContent(authoring.Content);

            return new EnemyDefinition(
                content);
        }

        private static List<ContentEntry> BuildContent(
            IReadOnlyList<EnemyVariantData> authoring)
        {
            List<ContentEntry> result =
                new(authoring.Count);

            foreach (EnemyVariantData asset in authoring)
            {
                if (asset == null)
                    throw new InvalidOperationException(
                        "EnemyAuthoring contains a null EnemyVariantData.");

                result.Add(
                    new ContentEntry(
                        asset.Id,
                        asset));
            }

            return result;
        }
    }
}