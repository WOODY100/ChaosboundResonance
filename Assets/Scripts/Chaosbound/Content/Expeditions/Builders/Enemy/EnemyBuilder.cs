using Chaosbound.Content.Expeditions.Authoring.Enemy;
using Chaosbound.Content.Expeditions.Authoring.Enemy.TacticalIdentity;
using Chaosbound.Content.Expeditions.Definitions.Enemy;
using Chaosbound.Content.Expeditions.Definitions.Enemy.TacticalIdentity;
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

            TacticalIdentityDefinition tacticalIdentity =
                BuildTacticalIdentity(authoring.TacticalIdentity);

            return new EnemyDefinition(
                content,
                authoring.SchedulingPolicy,
                tacticalIdentity);
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

        private static TacticalIdentityDefinition BuildTacticalIdentity(
    TacticalIdentityAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<CapabilityAffinityDefinition> affinities =
                BuildAffinities(authoring.Affinities);

            return new TacticalIdentityDefinition(
                affinities);
        }

        private static List<CapabilityAffinityDefinition> BuildAffinities(
            IReadOnlyList<CapabilityAffinityAuthoring> authoring)
        {
            List<CapabilityAffinityDefinition> result =
                new(authoring.Count);

            foreach (CapabilityAffinityAuthoring affinity in authoring)
            {
                if (affinity == null)
                {
                    throw new InvalidOperationException(
                        "TacticalIdentityAuthoring contains a null CapabilityAffinityAuthoring.");
                }

                result.Add(
                    new CapabilityAffinityDefinition(
                        affinity.Capability,
                        affinity.BonusScore));
            }

            return result;
        }
    }
}