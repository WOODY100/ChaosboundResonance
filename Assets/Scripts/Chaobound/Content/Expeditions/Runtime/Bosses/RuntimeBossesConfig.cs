using Chaosbound.Shared.Identifiers;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Bosses
{
    public sealed class RuntimeBossesConfig
    {
        public RuntimeBossesConfig(
            IReadOnlyList<ContentReference> content)
        {
            Content = content;
        }

        public IReadOnlyList<ContentReference> Content { get; }
    }
}