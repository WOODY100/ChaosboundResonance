using Chaosbound.Gameplay.Pressure.ValueObjects;
using Chaosbound.Gameplay.Threat.ValueObjects;
using UnityEngine;

namespace Chaosbound.Gameplay.Threat.Policies
{
    /// <summary>
    /// Describes how pressure is converted into threat capacity.
    /// This asset contains only declarative data and exposes a simple
    /// evaluation API for the ThreatBudgetEvaluator.
    /// </summary>
    [CreateAssetMenu(
        fileName = "Threat Budget Policy",
        menuName = "Chaosbound/Threat/Budget Policy")]
    public sealed class ThreatBudgetPolicy : ScriptableObject
    {
        [Header("Threat Capacity")]

        [SerializeField]
        private AnimationCurve m_capacityCurve = AnimationCurve.Linear(
            0f, 0f,
            100f, 200f);

        /// <summary>
        /// Evaluates the threat capacity for the specified pressure.
        /// </summary>
        /// <param name="pressure">
        /// Current expedition pressure.
        /// </param>
        /// <returns>
        /// The evaluated threat capacity.
        /// </returns>
        public ThreatCapacity Evaluate(
            PressureValue pressure)
        {
            float pressureValue = Mathf.Max(
                0f,
                pressure.Value);

            float capacity = m_capacityCurve.Evaluate(
                pressureValue);

            capacity = Mathf.Max(
                0f,
                capacity);

            return new ThreatCapacity(
                capacity);
        }
    }
}