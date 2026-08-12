using Chaosbound.Content.Expeditions.Runtime.Combat;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Combat.Services
{
    /// <summary>
    /// Evaluates the current combat target from the configured
    /// target progression and the maximum target of the active tactic.
    ///
    /// This service is responsible only for converting the normalized
    /// progression into an effective combat target.
    /// </summary>
    public sealed class CombatTargetEvaluator
    {
        /// <summary>
        /// Evaluates the current combat target.
        /// </summary>
        /// <param name="progression">
        /// Runtime target progression configuration.
        /// </param>
        /// <param name="maximumTarget">
        /// Maximum target configured by the active combat tactic.
        /// </param>
        /// <param name="elapsedSeconds">
        /// Seconds elapsed since the expedition started.
        /// </param>
        /// <returns>
        /// The effective combat target for the current moment.
        /// </returns>
        public int Evaluate(
            RuntimeCombatTargetProgression progression,
            int maximumTarget,
            float elapsedSeconds)
        {
            if (progression == null)
            {
                throw new ArgumentNullException(
                    nameof(progression));
            }

            if (maximumTarget < 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumTarget),
                    maximumTarget,
                    "Maximum target must be at least 3.");
            }

            float progress =
                progression.Profile.EvaluateProgress(
                    elapsedSeconds);

            int target =
                Mathf.RoundToInt(
                    maximumTarget * progress);

            return Mathf.Clamp(
                target,
                3,
                maximumTarget);
        }
    }
}