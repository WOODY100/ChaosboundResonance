using System;
using Chaosbound.Gameplay.Spawn.References;
using Chaosbound.Shared.Contracts;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates typed materializable references
    /// from runtime content.
    /// </summary>
    public sealed class MaterializableReferenceFactory
    {
        /// <summary>
        /// Creates a materializable reference
        /// for the supplied enemy content.
        /// </summary>
        public IMaterializableReference Create(
            EnemyVariantData enemy)
        {
            if (enemy == null)
                throw new ArgumentNullException(nameof(enemy));

            return new EnemyMaterializableReference(enemy);
        }
    }
}