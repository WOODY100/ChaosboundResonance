using System;
using Chaosbound.Content.Expeditions.Authoring.Threat;
using Chaosbound.Content.Expeditions.Definitions.Threat;

namespace Chaosbound.Content.Expeditions.Builders.Threat
{
    /// <summary>
    /// Converts threat authoring into its domain representation.
    /// </summary>
    public static class ThreatBuilder
    {
        public static ThreatDefinition Build(
            ThreatAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            return new ThreatDefinition(
                authoring.BudgetPolicy);
        }
    }
}