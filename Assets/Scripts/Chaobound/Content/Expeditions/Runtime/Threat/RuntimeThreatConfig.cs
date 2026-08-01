using System;
using Chaosbound.Gameplay.Threat.Policies;

namespace Chaosbound.Content.Expeditions.Runtime.Threat
{
    /// <summary>
    /// Runtime configuration for expedition threat settings.
    /// </summary>
    public sealed class RuntimeThreatConfig
    {
        public ThreatBudgetPolicy BudgetPolicy { get; }

        public RuntimeThreatConfig(
            ThreatBudgetPolicy budgetPolicy)
        {
            BudgetPolicy = budgetPolicy
                ?? throw new ArgumentNullException(nameof(budgetPolicy));
        }
    }
}