using Chaosbound.Shared.Content.Entries;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Enemy
{
    /// <summary>
    /// Defines the content catalog available for an expedition.
    /// </summary>
    public sealed class EnemyDefinition
    {
        /// <summary>
        /// Gets the content available for this expedition.
        /// </summary>
        public IReadOnlyList<ContentEntry> Entries { get; }

        public EnemyDefinition(
            IReadOnlyList<ContentEntry> entries)
        {
            Entries = new List<ContentEntry>(entries);
        }
    }
}