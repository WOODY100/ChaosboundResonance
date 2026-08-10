using Chaosbound.Content.Expeditions.Enums.Enemy;
using Chaosbound.Shared.Enums;
using System;

namespace Chaosbound.Gameplay.Combat.Planning
{
    /// <summary>
    /// Represents the concrete replenishment planning information
    /// required to resolve enemy variants for materialization.
    ///
    /// This object does not select an enemy variant and does not
    /// interact with the Spawn Runtime.
    /// </summary>
    public sealed class CombatReplenishmentPlan
    {
        /// <summary>
        /// Gets the enemy role that must be replenished.
        /// </summary>
        public EnemyRole Role { get; }

        /// <summary>
        /// Gets the number of enemies that must be replenished.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Gets the enemy tier used when resolving the enemy pool.
        /// </summary>
        public EnemyTier Tier { get; }

        public CombatReplenishmentPlan(
            EnemyRole role,
            int quantity,
            EnemyTier tier)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Replenishment quantity must be greater than zero.");
            }

            Role = role;
            Quantity = quantity;
            Tier = tier;
        }
    }
}