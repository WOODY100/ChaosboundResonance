using Chaosbound.Shared.Content.Entries;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Bosses
{
    public sealed class BossesDefinition
    {
        public BossesDefinition(
            IReadOnlyList<ContentEntry> entries)
        {
            Entries =
                new List<ContentEntry>(entries);
        }

        public IReadOnlyList<ContentEntry> Entries { get; }
    }
}