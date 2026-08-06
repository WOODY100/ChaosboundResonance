using Chaosbound.Gameplay.EnemySolver.Enums;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Enemy.TacticalIdentity
{
    /// <summary>
    /// Immutable runtime representation of an expedition tactical identity.
    /// Consumed by the EnemySolver evaluation pipeline.
    /// </summary>
    public sealed class RuntimeTacticalIdentity
    {
        private readonly Dictionary<TacticalCapability, float> bonusLookup;

        private readonly Dictionary<TacticalCapability, int>
            desiredCountLookup;

        /// <summary>
        /// Gets every tactical affinity configured for the expedition.
        /// </summary>
        public IReadOnlyList<RuntimeCapabilityAffinity> Affinities { get; }

        public RuntimeTacticalIdentity(
            IReadOnlyList<RuntimeCapabilityAffinity> affinities)
        {
            Affinities =
                affinities ??
                throw new ArgumentNullException(nameof(affinities));

            bonusLookup =
                new Dictionary<TacticalCapability, float>(
                    affinities.Count);

            desiredCountLookup =
                new Dictionary<TacticalCapability, int>(
                    affinities.Count);

            foreach (RuntimeCapabilityAffinity affinity in affinities)
            {
                bonusLookup.Add(
                    affinity.Capability,
                    affinity.BonusScore);

                desiredCountLookup.Add(
                    affinity.Capability,
                    affinity.DesiredCount);
            }
        }

        /// <summary>
        /// Gets the configured bonus score for the specified tactical capability.
        /// Returns zero when no affinity has been configured.
        /// </summary>
        public float GetBonusScore(
            TacticalCapability capability)
        {
            return bonusLookup.TryGetValue(
                capability,
                out float bonusScore)
                ? bonusScore
                : 0f;
        }

        /// <summary>
        /// Gets the desired alive count for the specified tactical capability.
        /// Returns zero when no affinity has been configured.
        /// </summary>
        public int GetDesiredCount(
            TacticalCapability capability)
        {
            return desiredCountLookup.TryGetValue(
                capability,
                out int desiredCount)
                ? desiredCount
                : 0;
        }
    }
}