using Chaosbound.Shared.Enums;
using System;

namespace Chaosbound.Gameplay.Combat.Models
{
    /// <summary>
    /// Represents the desired quantity of enemies assigned
    /// to a specific tactical role within a combat composition.
    /// </summary>
    public sealed class CombatRuntimeCompositionEntry
    {
        /// <summary>
        /// Gets the tactical role represented by this entry.
        /// </summary>
        public EnemyRole Role { get; }

        /// <summary>
        /// Gets the desired number of alive enemies
        /// for this role.
        /// </summary>
        public int TargetQuantity { get; }

        /// <summary>
        /// Creates a new combat composition entry.
        /// </summary>
        public CombatRuntimeCompositionEntry(
            EnemyRole role,
            int targetQuantity)
        {
            ValidateTargetQuantity(targetQuantity);

            Role = role;
            TargetQuantity = targetQuantity;
        }

        private static void ValidateTargetQuantity(
            int targetQuantity)
        {
            if (targetQuantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(targetQuantity),
                    targetQuantity,
                    "MaximumTarget quantity must be greater than zero.");
            }
        }
    }
}