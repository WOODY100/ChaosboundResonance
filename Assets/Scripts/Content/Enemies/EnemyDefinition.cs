using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Enemies
{
    /// <summary>
    /// Describes an enemy available in the game.
    /// </summary>
    public sealed class EnemyDefinition
    {
        public EnemyIdentity Identity { get; }

        public IReadOnlyList<CombatRole> CombatRoles { get; }

        public EnemyTier Tier { get; }

        public MovementType MovementType { get; }

        public EnemyDefinition(
            EnemyIdentity identity,
            IReadOnlyList<CombatRole> combatRoles,
            EnemyTier tier,
            MovementType movementType)
        {
            Identity = identity ??
                throw new ArgumentNullException(nameof(identity));

            CombatRoles = combatRoles ??
                throw new ArgumentNullException(nameof(combatRoles));

            Tier = tier;

            MovementType = movementType;
        }
    }
}