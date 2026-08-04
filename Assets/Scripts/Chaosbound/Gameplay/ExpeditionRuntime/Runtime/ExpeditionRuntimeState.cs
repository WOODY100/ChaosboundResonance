using Chaosbound.Gameplay.Pressure.ValueObjects;
using Chaosbound.Gameplay.Threat.Runtime;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Runtime
{
    /// <summary>
    /// Represents the mutable runtime state
    /// of the current expedition.
    /// </summary>
    public sealed class ExpeditionRuntimeState
    {
        /// <summary>
        /// Gets the elapsed expedition time.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current expedition pressure.
        /// </summary>
        public PressureValue CurrentPressure
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current expedition threat budget.
        /// </summary>
        public ThreatBudget ThreatBudget
        {
            get;
            private set;
        }

        /// <summary>
        /// Advances the runtime clock.
        /// </summary>
        public void AdvanceTime(
            TimeSpan deltaTime)
        {
            ElapsedTime += deltaTime;
        }

        /// <summary>
        /// Updates the current expedition pressure.
        /// </summary>
        public void SetPressure(
            PressureValue pressure)
        {
            CurrentPressure = pressure;
        }

        /// <summary>
        /// Sets the current expedition threat budget.
        /// </summary>
        public void SetThreatBudget(
            ThreatBudget threatBudget)
        {
            ThreatBudget = threatBudget
                ?? throw new ArgumentNullException(
                    nameof(threatBudget));
        }
    }
}