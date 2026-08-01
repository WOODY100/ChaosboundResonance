using Chaosbound.Gameplay.Pressure.ValueObjects;
using UnityEngine;

namespace Chaosbound.Gameplay.Pressure.Profiles
{
    /// <summary>
    /// Describes how pressure evolves over time during an expedition.
    /// This asset contains only declarative data and exposes a simple
    /// evaluation API for the PressureEvaluator.
    /// </summary>
    [CreateAssetMenu(
        fileName = "Pressure Curve Profile",
        menuName = "Chaosbound/Pressure/Curve Profile")]
    public sealed class PressureCurveProfile : ScriptableObject
    {
        [Header("Pressure")]

        [SerializeField]
        private AnimationCurve m_pressureCurve = AnimationCurve.Linear(
            0f, 0f,
            600f, 100f);

        /// <summary>
        /// Evaluates the pressure for the given elapsed time.
        /// </summary>
        /// <param name="elapsedSeconds">
        /// Seconds elapsed since the expedition started.
        /// </param>
        /// <returns>
        /// The evaluated pressure value.
        /// </returns>
        public PressureValue Evaluate(
            float elapsedSeconds)
        {
            elapsedSeconds = Mathf.Max(0f, elapsedSeconds);

            float pressure = m_pressureCurve.Evaluate(
                elapsedSeconds);

            pressure = Mathf.Max(0f, pressure);

            return new PressureValue(
                pressure);
        }
    }
}