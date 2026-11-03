using Chaosbound.Content.Expeditions.Definitions.Timeline;
using Chaosbound.Gameplay.Combat.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using Chaosbound.Gameplay.Timeline;
using System;

namespace Chaosbound.Gameplay.Combat.Director
{
    /// <summary>
    /// Interprets Timeline entries owned by Combat and updates
    /// the active enemy tier for future replenishment plans.
    /// </summary>
    public sealed class CombatTierProgressionDirector
    {
        private const string CombatDomainId = "combat";
        private const string TierProgressionContentId = "tier_progression";

        public void Update(
            CombatRuntimeState combatState,
            CompletionRuntimeState completionState,
            TimelineRuntimeState timelineState)
        {
            if (combatState == null)
                throw new ArgumentNullException(nameof(combatState));

            if (completionState == null)
                throw new ArgumentNullException(nameof(completionState));

            if (timelineState == null)
                throw new ArgumentNullException(nameof(timelineState));

            // Completion has priority over intermediate timeline steps.
            if (completionState.State ==
                CompletionDomainState.Completed)
            {
                combatState.ActivateFinalEnemyTier();
                return;
            }

            foreach (TimelineEntry entry in
                timelineState.ReachedEntriesThisTick)
            {
                if (entry == null)
                    continue;

                TimelineTriggerReference trigger =
                    entry.TriggerReference;

                if (trigger == null)
                    continue;

                if (!string.Equals(
                    trigger.DomainId,
                    CombatDomainId,
                    StringComparison.Ordinal))
                {
                    continue;
                }

                if (!string.Equals(
                    trigger.ContentId,
                    TierProgressionContentId,
                    StringComparison.Ordinal))
                {
                    continue;
                }

                combatState.AdvanceEnemyTier();
            }
        }
    }
}
