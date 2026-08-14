using Chaosbound.Gameplay.Combat.Runtime;
using Chaosbound.Gameplay.Combat.Runtime.Composition;
using Chaosbound.Gameplay.Timeline;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Runtime
{
    /// <summary>
    /// Represents the mutable runtime state
    /// of the current expedition.
    /// </summary>
    public sealed class ExpeditionRuntimeState
    {
        private readonly CombatRuntimeComposition
            runtimeComposition =
                new CombatRuntimeComposition();

        private readonly CombatRuntimeState
            combatRuntime =
                new CombatRuntimeState();

        private readonly TimelineRuntimeState
            timelineRuntime =
                new TimelineRuntimeState();

        public CombatRuntimeComposition RuntimeComposition =>
            runtimeComposition;

        /// <summary>
        /// Gets the delta time applied during the latest
        /// expedition runtime tick.
        /// </summary>
        public TimeSpan DeltaTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the elapsed expedition time.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current combat runtime state.
        /// </summary>
        public CombatRuntimeState Combat =>
            combatRuntime;

        /// <summary>
        /// Gets the current timeline runtime state.
        /// </summary>
        public TimelineRuntimeState Timeline =>
            timelineRuntime;

        /// <summary>
        /// Advances the runtime clock.
        /// </summary>
        public void AdvanceTime(
            TimeSpan deltaTime)
        {
            if (deltaTime < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deltaTime),
                    "Delta time cannot be negative.");
            }

            DeltaTime = deltaTime;

            ElapsedTime += deltaTime;
        }
    }
}