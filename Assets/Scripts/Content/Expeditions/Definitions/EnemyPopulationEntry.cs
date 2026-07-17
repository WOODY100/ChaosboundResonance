using Chaosbound.Content.Enemies;
using System;

namespace Chaosbound.Content.Expeditions.Definitions
{
    /// <summary>
    /// Describes how an enemy participates in an expedition.
    /// </summary>
    public sealed class EnemyPopulationEntry
    {
        /// <summary>
        /// Enemy referenced by this population entry.
        /// </summary>
        public EnemyIdentity Enemy { get; }

        public EnemyPopulationEntry(
            EnemyIdentity enemy)
        {
            Enemy = enemy ??
                throw new ArgumentNullException(nameof(enemy));
        }
    }
}