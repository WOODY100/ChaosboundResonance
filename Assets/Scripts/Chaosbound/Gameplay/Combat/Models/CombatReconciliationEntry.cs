using Chaosbound.Shared.Enums;
using System;

namespace Chaosbound.Gameplay.Combat.Models
{
    /// <summary>
    /// Represents the reconciliation state for a single
    /// combat type and enemy role within the current combat.
    /// </summary>
    public sealed class CombatReconciliationEntry
    {
        /// <summary>
        /// Gets the combat type represented by this entry.
        /// </summary>
        public EnemyCombatType CombatType { get; }

        /// <summary>
        /// Gets the enemy role represented by this entry.
        /// </summary>
        public EnemyRole Role { get; }

        /// <summary>
        /// Gets the desired number of enemies for this
        /// combat type and role.
        /// </summary>
        public int TargetQuantity { get; }

        /// <summary>
        /// Gets the current number of alive enemies for this
        /// combat type and role.
        /// </summary>
        public int CurrentQuantity { get; }

        /// <summary>
        /// Gets the number of enemies missing from the target.
        /// </summary>
        public int MissingQuantity { get; }

        /// <summary>
        /// Gets whether this combat type and role requires
        /// replenishment.
        /// </summary>
        public bool RequiresReplenishment =>
            MissingQuantity > 0;

        public CombatReconciliationEntry(
            EnemyCombatType combatType,
            EnemyRole role,
            int targetQuantity,
            int currentQuantity)
        {
            if (targetQuantity < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(targetQuantity));
            }

            if (currentQuantity < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(currentQuantity));
            }

            CombatType =
                combatType;

            Role =
                role;

            TargetQuantity =
                targetQuantity;

            CurrentQuantity =
                currentQuantity;

            MissingQuantity =
                Math.Max(
                    0,
                    targetQuantity - currentQuantity);
        }
    }
}