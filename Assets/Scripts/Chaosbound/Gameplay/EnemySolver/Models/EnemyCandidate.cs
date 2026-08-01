using Chaosbound.Gameplay.EnemySolver.Enums;
using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Threat.ValueObjects;

namespace Chaosbound.Gameplay.EnemySolver.Models
{
    /// <summary>
    /// Represents a possible enemy variant that can participate in the
    /// composition currently being built by the EnemySolver.
    /// </summary>
    public sealed class EnemyCandidate
    {
        /// <summary>
        /// Gets the underlying enemy variant.
        /// </summary>
        public EnemyVariantData Variant { get; }

        /// <summary>
        /// Gets the threat cost of the candidate.
        /// </summary>
        public ThreatCost ThreatCost => Variant.ThreatCost;

        /// <summary>
        /// Gets the category of the candidate.
        /// </summary>
        public EnemyCategory Category => Variant.Category;

        /// <summary>
        /// Gets the tactical roles provided by this candidate.
        /// </summary>
        public IReadOnlyList<EnemyRole> Roles => Variant.Roles;

        /// <summary>
        /// Gets the tactical capabilities provided by this candidate.
        /// </summary>
        public IReadOnlyList<TacticalCapability> TacticalCapabilities =>
            Variant.TacticalCapabilities;

        /// <summary>
        /// Creates a new candidate.
        /// </summary>
        public EnemyCandidate(
            EnemyVariantData variant)
        {
            Variant = variant
                ?? throw new ArgumentNullException(nameof(variant));
        }
    }
}