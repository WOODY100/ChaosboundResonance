using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Content.Expeditions.Runtime.Bosses;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Content.Expeditions.Definitions.Timeline;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Coordinates Boss Domain runtime behavior
    /// for the current expedition.
    /// </summary>
    public sealed class BossDomainDirector
    {
        private const string BossDomainId =
            "boss";

        private const string BossStartContentId =
            "boss.start";

        /// <summary>
        /// Executes the Boss Domain for the current
        /// Expedition Runtime tick.
        /// </summary>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            IReadOnlyList<TimelineEntry> reachedEntries =
                context.State
                    .Timeline
                    .ReachedEntriesThisTick;

            foreach (TimelineEntry entry in reachedEntries)
            {
                if (entry == null)
                    continue;

                if (entry.TriggerReference == null)
                    continue;

                if (entry.TriggerReference.DomainId !=
                    BossDomainId)
                {
                    continue;
                }

                HandleBossTrigger(
                    context,
                    entry);
            }
        }

        private void HandleBossTrigger(
            ExpeditionRuntimeContext context,
            TimelineEntry entry)
        {
            if (entry.TriggerReference.ContentId !=
                BossStartContentId)
            {
                throw new InvalidOperationException(
                    $"Unsupported Boss trigger ContentId: " +
                    $"'{entry.TriggerReference.ContentId}'.");
            }

            if (context.State.Boss.State !=
                BossDomainState.Inactive)
            {
                return;
            }

            BossData selectedBoss =
                SelectBoss(
                    context.Config.Bosses);

            context.State.Boss.Start(
                selectedBoss);
        }

        private BossData SelectBoss(
            RuntimeBossesConfig runtimeBossesConfig)
        {
            if (runtimeBossesConfig == null)
            {
                throw new InvalidOperationException(
                    "Boss Domain requires " +
                    "RuntimeBossesConfig.");
            }

            IReadOnlyList<BossData> bosses =
                runtimeBossesConfig.Bosses;

            if (bosses == null ||
                bosses.Count == 0)
            {
                throw new InvalidOperationException(
                    "Boss Domain cannot start because " +
                    "the expedition contains no available Bosses.");
            }

            // Temporary deterministic selection.
            // The V1 Boss selection policy will be defined
            // independently from the Timeline Domain.
            return bosses[0];
        }
    }
}