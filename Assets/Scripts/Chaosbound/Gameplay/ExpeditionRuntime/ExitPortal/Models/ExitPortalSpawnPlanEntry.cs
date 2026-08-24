using Chaosbound.Content.Portal.Exit;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Models
{
    /// <summary>
    /// Represents an Exit Portal materialization entry
    /// produced by the Exit Portal Domain.
    /// </summary>
    public sealed class ExitPortalSpawnPlanEntry
    {
        /// <summary>
        /// Gets the Exit Portal that should be materialized.
        /// </summary>
        public ExitPortalData ExitPortal { get; }

        /// <summary>
        /// Gets the quantity requested for materialization.
        /// </summary>
        public int Quantity { get; }

        public ExitPortalSpawnPlanEntry(
            ExitPortalData exitPortal,
            int quantity)
        {
            ExitPortal =
                exitPortal
                ?? throw new ArgumentNullException(
                    nameof(exitPortal));

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Exit Portal spawn quantity must be greater than zero.");
            }

            Quantity =
                quantity;
        }
    }
}