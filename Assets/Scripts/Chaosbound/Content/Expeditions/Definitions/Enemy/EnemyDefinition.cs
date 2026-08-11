using Chaosbound.Content.Expeditions.Enums.Enemy;
using Chaosbound.Shared.Content.Entries;
using System;
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

        public EnemySchedulingPolicy SchedulingPolicy { get; }

        public EnemyDefinition(
            IReadOnlyList<ContentEntry> entries,
            EnemySchedulingPolicy schedulingPolicy)
        {
            Entries = new List<ContentEntry>(entries);

            SchedulingPolicy = schedulingPolicy;
        }
    }
}