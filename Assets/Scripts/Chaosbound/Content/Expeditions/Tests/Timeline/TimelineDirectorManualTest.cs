using Chaosbound.Content.Expeditions.Definitions.Timeline;
using Chaosbound.Content.Expeditions.Directors.Timeline;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Tests.Timeline
{
    public sealed class TimelineDirectorManualTest : MonoBehaviour
    {
        [ContextMenu("Run Timeline Director Test")]
        private void RunTest()
        {
            TimelineContent content =
                CreateTestContent();

            TimelineAgenda agenda =
                TimelineDirector.Build(content);

            Debug.Log(
                $"[Timeline Test] " +
                $"Completion Target: {agenda.CompletionTargetTime}");

            Debug.Log(
                $"[Timeline Test] " +
                $"Entry Count: {agenda.Entries.Count}");

            for (int i = 0; i < agenda.Entries.Count; i++)
            {
                TimelineEntry entry =
                    agenda.Entries[i];

                Debug.Log(
                    $"[Timeline Test] " +
                    $"[{i}] " +
                    $"EntryId={entry.EntryId} | " +
                    $"EventId={entry.EventId} | " +
                    $"Time={entry.ScheduledTime} | " +
                    $"IconId={entry.IconId} | " +
                    $"Trigger=" +
                    FormatTrigger(entry.TriggerReference));
            }

            ValidateAgenda(agenda);
        }

        private static TimelineContent CreateTestContent()
        {
            List<TimelineEventDefinition> definitions =
                new()
                {
                    new TimelineEventDefinition(
                        "boss",
                        "icon.boss",
                        new TimelineTriggerReference(
                            "boss",
                            "boss.test_boss")),

                    new TimelineEventDefinition(
                        "dynamic_event",
                        "icon.dynamic",
                        new TimelineTriggerReference(
                            "dynamic_event",
                            "event.test_dynamic")),

                    new TimelineEventDefinition(
                        "npc_rescue",
                        "icon.npc",
                        null),

                    new TimelineEventDefinition(
                        "marker",
                        "icon.marker",
                        null)
                };

            List<FixedTimeRule> fixedRules =
                new()
                {
                    new FixedTimeRule(
                        "boss",
                        1200f)
                };

            List<DistributedRule> distributedRules =
                new()
                {
                    new DistributedRule(
                        "dynamic_event",
                        3,
                        300f,
                        TimelineTimeReference.CompletionTarget())
                };

            List<ExplicitTimelineEvent> explicitEvents =
                new()
                {
                    new ExplicitTimelineEvent(
                        "npc_rescue",
                        750f)
                };

            ExpeditionCompletionTarget completionTarget =
                new(1500f);

            return new TimelineContent(
                definitions,
                fixedRules,
                distributedRules,
                explicitEvents,
                completionTarget);
        }

        private static void ValidateAgenda(
            TimelineAgenda agenda)
        {
            if (agenda.CompletionTargetTime != 1500f)
                throw new System.Exception(
                    "Completion target is incorrect.");

            if (agenda.Entries.Count != 5)
                throw new System.Exception(
                    "Expected exactly 5 timeline entries.");

            AssertEntry(
                agenda.Entries[0],
                "distributed:0:0",
                "dynamic_event",
                300f);

            AssertEntry(
                agenda.Entries[1],
                "explicit:0",
                "npc_rescue",
                750f);

            AssertEntry(
                agenda.Entries[2],
                "distributed:0:1",
                "dynamic_event",
                900f);

            AssertEntry(
                agenda.Entries[3],
                "fixed:0",
                "boss",
                1200f);

            AssertEntry(
                agenda.Entries[4],
                "distributed:0:2",
                "dynamic_event",
                1500f);

            TimelineEntry boss =
                agenda.Entries[3];

            if (boss.TriggerReference == null)
                throw new System.Exception(
                    "Boss trigger reference is missing.");

            if (boss.TriggerReference.DomainId != "boss")
                throw new System.Exception(
                    "Boss DomainId is incorrect.");

            if (boss.TriggerReference.ContentId !=
                "boss.test_boss")
            {
                throw new System.Exception(
                    "Boss ContentId is incorrect.");
            }

            TimelineEntry npc =
                agenda.Entries[1];

            if (npc.TriggerReference != null)
                throw new System.Exception(
                    "NPC rescue should be Marker-only.");

            Debug.Log(
                "[Timeline Test] ALL TESTS PASSED.");
        }

        private static void AssertEntry(
            TimelineEntry entry,
            string expectedEntryId,
            string expectedEventId,
            float expectedTime)
        {
            if (entry.EntryId != expectedEntryId)
                throw new System.Exception(
                    $"Expected EntryId '{expectedEntryId}', " +
                    $"got '{entry.EntryId}'.");

            if (entry.EventId != expectedEventId)
                throw new System.Exception(
                    $"Expected EventId '{expectedEventId}', " +
                    $"got '{entry.EventId}'.");

            if (!Mathf.Approximately(
                    entry.ScheduledTime,
                    expectedTime))
            {
                throw new System.Exception(
                    $"Expected time {expectedTime}, " +
                    $"got {entry.ScheduledTime}.");
            }
        }

        private static string FormatTrigger(
            TimelineTriggerReference trigger)
        {
            if (trigger == null)
                return "None";

            return
                $"{trigger.DomainId}:{trigger.ContentId}";
        }
    }
}