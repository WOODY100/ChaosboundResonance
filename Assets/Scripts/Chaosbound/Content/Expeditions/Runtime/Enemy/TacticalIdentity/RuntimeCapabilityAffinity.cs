using Chaosbound.Gameplay.EnemySolver.Enums;
using System;

namespace Chaosbound.Content.Expeditions.Runtime.Enemy.TacticalIdentity
{
    /// <summary>
    /// Immutable runtime representation of a tactical affinity.
    /// Used by the EnemySolver evaluation pipeline.
    /// </summary>
    public sealed class RuntimeCapabilityAffinity
    {
        /// <summary>
        /// Gets the favored tactical capability.
        /// </summary>
        public TacticalCapability Capability { get; }

        /// <summary>
        /// Gets the bonus score contributed during evaluation.
        /// </summary>
        public float BonusScore { get; }

        /// <summary>
        /// Gets the desired alive enemies contributing this capability.
        /// </summary>
        public int DesiredCount { get; }

        public RuntimeCapabilityAffinity(
            TacticalCapability capability,
            float bonusScore,
            int desiredCount)
        {
            if (desiredCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(desiredCount),
                    desiredCount,
                    "Desired count cannot be negative.");
            }

            Capability = capability;
            BonusScore = bonusScore;
            DesiredCount = desiredCount;
        }
    }
}