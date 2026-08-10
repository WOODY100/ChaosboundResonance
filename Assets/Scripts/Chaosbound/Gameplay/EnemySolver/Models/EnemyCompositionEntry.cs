using Chaosbound.Shared.Enums;
using Chaosbound.Gameplay.EnemySolver.Enums;
using Chaosbound.Gameplay.Threat.ValueObjects;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.EnemySolver.Models
{
    /// <summary>
    /// Represents a tactical allocation of a standard enemy variant
    /// within an immutable enemy composition.
    /// </summary>
    public sealed class EnemyCompositionEntry
    {
        /// <summary>
        /// Gets the enemy variant represented by this composition entry.
        /// </summary>
        public EnemyVariantData Variant { get; }

        /// <summary>
        /// Gets how many instances of this variant belong to the composition.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Gets the threat cost of a single unit.
        /// </summary>
        public ThreatCost ThreatCost => Variant.ThreatCost;

        /// <summary>
        /// Gets the total threat cost represented by this entry.
        /// </summary>
        public ThreatCost TotalThreatCost =>
            new ThreatCost(ThreatCost.Value * Quantity);

        /// <summary>
        /// Gets the tactical roles provided by this variant.
        /// </summary>
        public IReadOnlyList<EnemyRole> Roles => Variant.Roles;

        /// <summary>
        /// Gets the tactical capabilities provided by this variant.
        /// </summary>
        public IReadOnlyList<TacticalCapability> TacticalCapabilities =>
            Variant.TacticalCapabilities;

        /// <summary>
        /// Gets whether this entry represents a single enemy.
        /// </summary>
        public bool IsSingleUnit => Quantity == 1;

        /// <summary>
        /// Creates a new immutable composition entry.
        /// </summary>
        public EnemyCompositionEntry(
            EnemyVariantData variant,
            int quantity)
        {
            Variant = variant
                ?? throw new ArgumentNullException(nameof(variant));

            ValidateQuantity(quantity);

            Quantity = quantity;
        }

        /// <summary>
        /// Validates the quantity assigned to the composition entry.
        /// </summary>
        private static void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Quantity must be greater than zero.");
            }
        }
    }
}