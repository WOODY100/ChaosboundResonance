using Chaosbound.Content.Enemy.MiniBosses;
using System;

namespace Chaosbound.Gameplay.MiniBosses.Models
{
    /// <summary>
    /// Represents a concrete MiniBoss and the quantity
    /// requested for materialization by the MiniBoss Domain.
    /// </summary>
    public sealed class MiniBossSpawnPlanEntry
    {
        /// <summary>
        /// Gets the MiniBoss to materialize.
        /// </summary>
        public MiniBossData MiniBoss { get; }

        /// <summary>
        /// Gets the requested quantity.
        /// </summary>
        public int Quantity { get; }

        public MiniBossSpawnPlanEntry(
            MiniBossData miniBoss,
            int quantity)
        {
            MiniBoss =
                miniBoss
                ?? throw new ArgumentNullException(
                    nameof(miniBoss));

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "MiniBoss spawn quantity must be greater than zero.");
            }

            Quantity = quantity;
        }
    }
}