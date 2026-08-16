using Chaosbound.Content.Enemy.Bosses;
using System;

namespace Chaosbound.Gameplay.Bosses.Models
{
    /// <summary>
    /// Represents a Boss materialization entry
    /// produced by the Boss Domain.
    /// </summary>
    public sealed class BossSpawnPlanEntry
    {
        /// <summary>
        /// Gets the Boss that should be materialized.
        /// </summary>
        public BossData Boss { get; }

        /// <summary>
        /// Gets the quantity requested for materialization.
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