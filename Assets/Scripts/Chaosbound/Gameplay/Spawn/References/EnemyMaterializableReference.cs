using System;
using Chaosbound.Shared.Contracts;

namespace Chaosbound.Gameplay.Spawn.References
{
    /// <summary>
    /// Represents a typed reference to enemy content
    /// that can be materialized by the Spawn Runtime.
    /// </summary>
    public sealed class EnemyMaterializableReference :
        IMaterializableReference
    {
        /// <summary>
        /// Gets the enemy content referenced by this instance.
        /// </summary>
        public EnemyVariantData Enemy { get; }

        public EnemyMaterializableReference(
            EnemyVariantData enemy)
        {
            Enemy = enemy
                ?? throw new ArgumentNullException(nameof(enemy));
        }
    }
}