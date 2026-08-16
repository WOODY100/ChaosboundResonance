using Chaosbound.Content.Enemy.Bosses;
using System;

namespace Chaosbound.Gameplay.Bosses.Models
{
    /// <summary>
    /// Represents a concrete Boss and the quantity
    /// requested for materialization by the Boss Domain.
    /// </summary>
    public sealed class BossSpawnPlanEntry
    {
        /// <summary>
        /// Gets the concrete Boss to materialize.
        /// </summary>
        public BossData Boss { get; }

        /// <summary>
        /// Gets the requested quantity of this Boss.
        /// </summary>
        public int Quantity { get; }

        public BossSpawnPlanEntry(
            BossData boss,
            int quantity)
        {
            Boss =
                boss
                ?? throw new ArgumentNullException(
                    nameof(boss));

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Boss spawn quantity must be greater than zero.");
            }

            Quantity = quantity;
        }
    }
}