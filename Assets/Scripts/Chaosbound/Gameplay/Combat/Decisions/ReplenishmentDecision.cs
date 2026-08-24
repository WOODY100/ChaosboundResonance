using Chaosbound.Shared.Enums;
using System;

namespace Chaosbound.Gameplay.Combat.Decisions
{
    /// <summary>
    /// Represents a request to replenish the current
    /// combat population for a specific combat type and role.
    /// </summary>
    public readonly struct ReplenishmentDecision
    {
        public bool IsRequired { get; }

        public EnemyCombatType CombatType { get; }

        public EnemyRole Role { get; }

        public int Quantity { get; }

        private ReplenishmentDecision(
            bool isRequired,
            EnemyCombatType combatType,
            EnemyRole role,
            int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity));
            }

            IsRequired = isRequired;

            CombatType =
                combatType;

            Role =
                role;

            Quantity =
                quantity;
        }

        public static ReplenishmentDecision None =>
            new ReplenishmentDecision(
                false,
                EnemyCombatType.Melee,
                EnemyRole.Normal,
                0);

        public static ReplenishmentDecision Replenish(
            EnemyCombatType combatType,
            EnemyRole role,
            int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Replenishment quantity must be greater than zero.");
            }

            return new ReplenishmentDecision(
                true,
                combatType,
                role,
                quantity);
        }
    }
}