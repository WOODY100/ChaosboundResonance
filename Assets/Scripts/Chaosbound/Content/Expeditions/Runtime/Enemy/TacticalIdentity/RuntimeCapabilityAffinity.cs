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

        public RuntimeCapabilityAffinity(
            TacticalCapability capability,
            float bonusScore)
        {
            Capability = capability;
            BonusScore = bonusScore;
        }
    }
}