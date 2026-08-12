using Chaosbound.Content.Expeditions.Profiles.Combat;
using System;

namespace Chaosbound.Content.Expeditions.Definitions.Combat
{
    /// <summary>
    /// Defines the target progression configuration
    /// available for an expedition's Combat Domain.
    /// </summary>
    public sealed class CombatTargetProgressionDefinition
    {
        /// <summary>
        /// Gets the profile that defines how the combat target
        /// progresses over elapsed expedition time.
        /// </summary>
        public CombatTargetProgressionProfile Profile { get; }

        public CombatTargetProgressionDefinition(
            CombatTargetProgressionProfile profile)
        {
            Profile =
                profile
                ?? throw new ArgumentNullException(
                    nameof(profile));
        }
    }
}