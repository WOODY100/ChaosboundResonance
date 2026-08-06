using Chaosbound.Gameplay.EnemySolver.Analysis.Models;
using Chaosbound.Gameplay.EnemySolver.Enums;
using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.EnemySolver.Runtime.Composition;
using System;

namespace Chaosbound.Gameplay.EnemySolver.Analysis.Runtime
{
    /// <summary>
    /// Builds a tactical profile from the current runtime composition.
    /// </summary>
    public sealed class RuntimeTacticalProfileBuilder
    {
        /// <summary>
        /// Builds the tactical profile represented by the current runtime.
        /// </summary>
        public TacticalProfile Build(
            RuntimeCompositionState runtimeComposition)
        {
            if (runtimeComposition == null)
                throw new ArgumentNullException(nameof(runtimeComposition));

            TacticalProfile profile =
                new TacticalProfile();

            foreach (RuntimeCompositionEntry entry in runtimeComposition.Entries)
            {
                EnemyVariantData variant =
                    entry.Variant;

                foreach (TacticalCapability capability
                    in variant.TacticalCapabilities)
                {
                    profile.Increment(
                        capability,
                        entry.AliveCount);
                }
            }

            return profile;
        }
    }
}