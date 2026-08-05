using Chaosbound.Gameplay.EnemySolver.Enums;
using System;

namespace Chaosbound.Gameplay.EnemySolver.Analysis
{
    /// <summary>
    /// Represents the number of alive enemies providing
    /// a specific tactical capability.
    /// </summary>
    public sealed class TacticalCapabilityCount
    {
        /// <summary>
        /// Gets the tactical capability.
        /// </summary>
        public TacticalCapability Capability { get; }

        /// <summary>
        /// Gets the number of alive enemies providing the capability.
        /// </summary>
        public int Count { get; }

        public TacticalCapabilityCount(
            TacticalCapability capability,
            int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    "Capability count cannot be negative.");
            }

            Capability = capability;
            Count = count;
        }
    }
}