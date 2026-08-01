using UnityEngine;

namespace Chaosbound.Gameplay.Threat.Policies
{
    /// <summary>
    /// Describes how pressure is converted into threat capacity.
    /// This asset is interpreted by the ThreatBudgetEvaluator.
    /// </summary>
    [CreateAssetMenu(
        fileName = "Threat Budget Policy",
        menuName = "Chaosbound/Threat/Budget Policy")]
    public sealed class ThreatBudgetPolicy : ScriptableObject
    {
    }
}