using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Analyzes an enemy composition and produces its tactical profile.
    /// </summary>
    public sealed class CompositionAnalyzer
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

                foreach (TacticalCapability capability in variant.tacticalCapabilities)
                {
                    profile.Increment(capability, entry.Quantity);
                }
            }

            return profile;
        }
    }
}