using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Enemy.TacticalIdentity
{
    /// <summary>
    /// Immutable tactical identity describing the tactical
    /// preferences of an expedition.
    /// </summary>
    public sealed class TacticalIdentityDefinition
    {
        /// <summary>
        /// Gets every tactical affinity configured for the expedition.
        /// </summary>
        public IReadOnlyList<CapabilityAffinityDefinition> Affinities { get; }

        public TacticalIdentityDefinition(
            IReadOnlyList<CapabilityAffinityDefinition> affinities)
        {
            if (affinities == null)
            {
                throw new ArgumentNullException(nameof(affinities));
            }

            Affinities =
                new List<CapabilityAffinityDefinition>(affinities);
        }
    }
}