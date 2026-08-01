using System;
using Chaosbound.Gameplay.Pressure.ValueObjects;
using Chaosbound.Gameplay.Threat.Policies;
using Chaosbound.Gameplay.Threat.ValueObjects;

namespace Chaosbound.Gameplay.Threat.Services
{
    /// <summary>
    /// Evaluates the threat capacity for the current state of the expedition.
    /// </summary>
    public static class ThreatBudgetEvaluator
    {
        public static ThreatCapacity Evaluate(
            ThreatBudgetPolicy policy,
            PressureValue pressure)
        {
            if (policy == null)
                throw new ArgumentNullException(nameof(policy));

            return policy.Evaluate(pressure);
        }
    }
}