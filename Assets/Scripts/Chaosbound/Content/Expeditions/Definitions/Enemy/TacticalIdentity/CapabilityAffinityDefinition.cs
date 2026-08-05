using Chaosbound.Gameplay.EnemySolver.Enums;
using System;

namespace Chaosbound.Content.Expeditions.Definitions.Enemy.TacticalIdentity
{
    /// <summary>
    /// Immutable tactical affinity describing how much an expedition
    /// favors a specific tactical capability.
    /// </summary>
    public sealed class CapabilityAffinityDefinition
    {
        /// <summary>
        /// Gets the favored tactical capability.
        /// </summary>
        public TacticalCapability Capability { get; }

        /// <summary>
        /// Gets the bonus score contributed during candidate evaluation.
        /// </summary>
        public float BonusScore { get; }

        public CapabilityAffinityDefinition(
            TacticalCapability capability,
            float bonusScore)
        {
            if (!Enum.IsDefined(typeof(TacticalCapability), capability))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capability),
                    capability,
                    "Invalid tactical capability.");
            }

            if (bonusScore < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(bonusScore),
                    bonusScore,
                    "Bonus score cannot be negative.");
            }

            Capability = capability;
            BonusScore = bonusScore;
        }
    }
}