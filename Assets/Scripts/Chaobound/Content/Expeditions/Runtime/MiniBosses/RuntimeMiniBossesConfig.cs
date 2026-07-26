using Chaosbound.Shared.Identifiers;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.MiniBosses
{
    public sealed class RuntimeMiniBossesConfig
    {
        public RuntimeMiniBossesConfig(
            IReadOnlyList<ContentReference> content)
        {
            Content = content;
        }

        public IReadOnlyList<ContentReference> Content { get; }
    }
}