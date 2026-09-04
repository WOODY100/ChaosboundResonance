using Chaosbound.Gameplay.Bosses;
using Chaosbound.Gameplay.Combat.Runtime;
using Chaosbound.Gameplay.Combat.Runtime.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Interactions;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using Chaosbound.Gameplay.ExpeditionRuntime.References.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.References.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Statistics;
using Chaosbound.Gameplay.ExpeditionRuntime.XP.Runtime;
using Chaosbound.Gameplay.MiniBosses;
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

        private readonly ExpeditionModifierState
            modifierState =
                new ExpeditionModifierState();

        private readonly ExpeditionInteractableUsageState
            interactableUsageState =
                new ExpeditionInteractableUsageState();

        private readonly CombatRuntimeState
            combatRuntime =
                new CombatRuntimeState();

        private readonly TimelineRuntimeState
            timelineRuntime =
                new TimelineRuntimeState();

        private readonly BossRuntimeState
            bossRuntime =
                new BossRuntimeState();

        private readonly MiniBossRuntimeState
            miniBossRuntime =
                new MiniBossRuntimeState();

        private readonly CompletionRuntimeState
            completionRuntime =
                new CompletionRuntimeState();

        private readonly EventCompletionBuffer
            eventCompletionBuffer =
                new EventCompletionBuffer();

        private readonly ExitPortalRuntimeState
            exitPortalRuntime =
                new ExitPortalRuntimeState();

        private readonly IRuntimeReferenceRegistry
            runtimeReferences =
                new RuntimeReferenceRegistry();

        private readonly XPFragmentRuntimeState
            xpFragments =
                new XPFragmentRuntimeState();

        private readonly ExpeditionRuntimeStatistics
            statistics =
                new ExpeditionRuntimeStatistics();

        public CombatRuntimeComposition RuntimeComposition =>
            runtimeComposition;

        public ExpeditionModifierState Modifiers =>
            modifierState;

        public ExpeditionRuntimeStatistics Statistics =>
            statistics;

        /// <summary>
        /// Gets the runtime usage state of one-use
        /// interactables during the current expedition.
        /// </summary>
        public ExpeditionInteractableUsageState InteractableUsage =>
            interactableUsageState;

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
        /// Gets the current boss runtime state.
        /// </summary>
        public BossRuntimeState Boss =>
            bossRuntime;

        public MiniBossRuntimeState MiniBoss =>
            miniBossRuntime;

        public CompletionRuntimeState Completion =>
            completionRuntime;

        public EventCompletionBuffer EventCompletions =>
            eventCompletionBuffer;

        public ExitPortalRuntimeState ExitPortal =>
            exitPortalRuntime;

        public XPFragmentRuntimeState XPFragments =>
            xpFragments;

        /// <summary>
        /// Gets the runtime reference registry
        /// for the current expedition.
        /// </summary>
        public IRuntimeReferenceRegistry RuntimeReferences =>
            runtimeReferences;

        public void ReportEventCompleted(
            EventCompleted completedEvent)
        {
            eventCompletionBuffer.Add(
                completedEvent);
        }

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