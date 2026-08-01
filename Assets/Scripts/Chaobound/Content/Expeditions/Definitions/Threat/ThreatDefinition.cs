using System;
using Chaosbound.Gameplay.Threat.Policies;

namespace Chaosbound.Content.Expeditions.Definitions.Threat
{
    /// <summary>
    /// Describes the threat budget configuration used by an expedition.
    /// Contains only declarative data.
    /// </summary>
    public sealed class ThreatDefinition
    {
        public ThreatBudgetPolicy BudgetPolicy { get; }

        public ThreatDefinition(
            ThreatBudgetPolicy budgetPolicy)
        {
            BudgetPolicy = budgetPolicy
                ?? throw new ArgumentNullException(nameof(budgetPolicy));
        }
    }
}