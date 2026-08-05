using Chaosbound.Gameplay.EnemySolver.Analysis.Models;
using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.EnemySolver.Enums;
using System;

namespace Chaosbound.Gameplay.EnemySolver.Analysis.Services
{
    /// <summary>
    /// Analyzes an enemy composition and produces its tactical profile.
    /// </summary>
    public sealed class TacticalProfileBuilder
    {
        /// <summary>
        /// Generates the tactical profile represented by the specified enemy composition.
        /// </summary>
        public TacticalProfile Analyze(EnemyComposition composition)
        {
            if (composition == null)
            {
                throw new ArgumentNullException(nameof(composition));
            }

            TacticalProfile profile = new TacticalProfile();

            foreach (EnemyCompositionEntry entry in composition.Entries)
            {
                EnemyVariantData variant = entry.Variant;

                foreach (TacticalCapability capability in variant.TacticalCapabilities)
                {
                    profile.Increment(capability, entry.Quantity);
                }
            }

            return profile;
        }
    }
}