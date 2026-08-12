using Chaosbound.Content.Expeditions.Profiles.Combat;
using System;

namespace Chaosbound.Content.Expeditions.Runtime.Combat
{
    /// <summary>
    /// Runtime configuration describing how the combat target
    /// progresses over elapsed expedition time.
    /// </summary>
    public sealed class RuntimeCombatTargetProgression
    {
        /// <summary>
        /// Gets the profile used to evaluate target progression.
        /// </summary>
        public CombatTargetProgressionProfile Profile { get; }

        public RuntimeCombatTargetProgression(
            CombatTargetProgressionProfile profile)
        {
            Profile =
                profile
                ?? throw new ArgumentNullException(
                    nameof(profile));
        }
    }
}