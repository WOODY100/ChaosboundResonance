using Chaosbound.Content.Enemies;
using System;

namespace Chaosbound.Content.Expeditions.Definitions
{
    /// <summary>
    /// Represents one tactical requirement inside a combat formation.
    /// </summary>
    public sealed class CombatFormationEntry
    {
        /// <summary>
        /// Tactical role requested by the formation.
        /// </summary>
        public CombatRole Role { get; }

        /// <summary>
        /// Desired amount of enemies with this role.
        /// </summary>
        public int Quantity { get; }

        public CombatFormationEntry(
            CombatRole role,
            int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            Role = role;
            Quantity = quantity;
        }
    }
}