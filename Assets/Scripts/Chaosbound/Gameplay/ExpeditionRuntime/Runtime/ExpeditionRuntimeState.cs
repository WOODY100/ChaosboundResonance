using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.Pressure.ValueObjects;
using Chaosbound.Gameplay.Threat.Runtime;
using Chaosbound.Gameplay.Pressure.Models;
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
        /// Gets the latest pressure snapshot.
        /// </summary>
        public PressureSnapshot PressureSnapshot
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
        /// Gets the latest enemy solver result.
        /// </summary>
        public EnemySolverResult EnemySolverResult
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
        /// Updates the latest pressure snapshot.
        /// </summary>
        public void SetPressureSnapshot(
            PressureSnapshot snapshot)
        {
            PressureSnapshot =
                snapshot
                ?? throw new ArgumentNullException(
                    nameof(snapshot));
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

        /// <summary>
        /// Updates the latest enemy solver result.
        /// </summary>
        public void SetEnemySolverResult(
            EnemySolverResult result)
        {
            EnemySolverResult =
                result
                ?? throw new ArgumentNullException(
                    nameof(result));
        }
    }
}