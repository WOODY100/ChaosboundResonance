using Chaosbound.Shared.Enums;
using System;

namespace Chaosbound.Gameplay.Combat.Decisions
{
    /// <summary>
    /// Represents a request to replenish the current
    /// combat population.
    /// </summary>
    public readonly struct ReplenishmentDecision
    {
        public bool IsRequired { get; }

        public EnemyRole Role { get; }

        public int Quantity { get; }

        private ReplenishmentDecision(
            bool isRequired,
            EnemyRole role,
            int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity));
            }

            IsRequired = isRequired;
            Role = role;
            Quantity = quantity;
        }

        public static ReplenishmentDecision None =>
            new ReplenishmentDecision(
                false,
                EnemyRole.Normal,
                0);

        public static ReplenishmentDecision Replenish(
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
                role,
                quantity);
        }
    }
}