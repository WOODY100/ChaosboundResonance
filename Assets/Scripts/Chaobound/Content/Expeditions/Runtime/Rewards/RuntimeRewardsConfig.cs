using Chaosbound.Shared.Identifiers;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Rewards
{
    public sealed class RuntimeRewardsConfig
    {
        public RuntimeRewardsConfig(
            IReadOnlyList<ContentReference> content)
        {
            Content = content;
        }

        public IReadOnlyList<ContentReference> Content { get; }
    }
}