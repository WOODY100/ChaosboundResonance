using System;
using UnityEngine;
using Chaosbound.Gameplay.Threat.Policies;

namespace Chaosbound.Content.Expeditions.Authoring.Threat
{
    /// <summary>
    /// Authoring configuration for the expedition threat settings.
    /// </summary>
    [Serializable]
    public sealed class ThreatAuthoring
    {
        [SerializeField]
        private ThreatBudgetPolicy m_budgetPolicy;

        public ThreatBudgetPolicy BudgetPolicy
        {
            get { return m_budgetPolicy; }
        }
    }
}