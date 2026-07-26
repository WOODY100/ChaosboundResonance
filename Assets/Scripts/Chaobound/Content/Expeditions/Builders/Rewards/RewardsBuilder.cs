using Chaosbound.Content.Expeditions.Authoring.Rewards;
using Chaosbound.Content.Expeditions.Definitions.Rewards;
using Chaosbound.Shared.Authoring;
using Chaosbound.Shared.Builders;
using Chaosbound.Shared.Identifiers;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.Rewards
{
    public static class RewardsBuilder
    {
        public static RewardsDefinition Build(
            RewardsAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            var content = new List<ContentReference>();

            foreach (var entry in authoring.Content ?? Array.Empty<ContentReferenceAuthoring>())
            {
                content.Add(ContentReferenceBuilder.Build(entry));
            }

            return new RewardsDefinition(content);
        }
    }
}