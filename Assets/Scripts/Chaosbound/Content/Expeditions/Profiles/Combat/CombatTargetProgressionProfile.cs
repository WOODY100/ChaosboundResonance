using UnityEngine;

namespace Chaosbound.Content.Expeditions.Profiles.Combat
{
    /// <summary>
    /// Defines the normalized progression of the combat target
    /// over the elapsed expedition time.
    ///
    /// The curve returns a normalized value where:
    /// 0 represents 0% of the MaximumTarget.
    /// 1 represents 100% of the MaximumTarget.
    /// </summary>
    [CreateAssetMenu(
        fileName = "Combat Target Progression Profile",
        menuName = "Chaosbound/Combat/Target Progression Profile")]
    public sealed class CombatTargetProgressionProfile :
        ScriptableObject
    {
        [Header("Target Progression")]

        [SerializeField]
        private AnimationCurve m_ProgressionCurve =
            AnimationCurve.Linear(
                0f,
                0f,
                1f,
                1f);

        /// <summary>
        /// Evaluates the normalized target progression
        /// for the supplied elapsed expedition time.
        /// </summary>
        /// <param name="elapsedSeconds">
        /// Seconds elapsed since the expedition started.
        /// </param>
        /// <returns>
        /// A normalized progression value between 0 and 1.
        /// </returns>
        public float EvaluateProgress(
            float elapsedSeconds)
        {
            elapsedSeconds =
                Mathf.Max(
                    0f,
                    elapsedSeconds);

            float progression =
                m_ProgressionCurve.Evaluate(
                    elapsedSeconds);

            return Mathf.Clamp01(
                progression);
        }
    }
}