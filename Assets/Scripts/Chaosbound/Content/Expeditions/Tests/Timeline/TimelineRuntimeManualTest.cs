using Chaosbound.Content.Expeditions.Definitions.Timeline;
using Chaosbound.Gameplay.Timeline;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Timeline.Tests
{
    public sealed class TimelineRuntimeManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run Timeline Runtime Test")]
        private void RunTest()
        {
            TimelineAgenda agenda =
                CreateTestAgenda();

            TimelineRuntime runtime =
                new TimelineRuntime();

            TimelineRuntimeState state =
                new TimelineRuntimeState();

            TestBeforeFirstEntry(
                runtime,
                agenda,
                state);

            TestMultipleEntries(
                runtime,
                agenda,
                state);

            TestJumpAcrossMultipleEntries(
                runtime,
                agenda,
                state);

            TestCompletionTarget(
                runtime,
                agenda,
                state);

            TestAfterCompletionTarget(
                runtime,
                agenda,
                state);

            Debug.Log(
                "[Timeline Runtime Test] ALL TESTS PASSED.");
        }

        private static TimelineAgenda CreateTestAgenda()
        {
            List<TimelineEntry> entries =
                new()
                {
                    CreateEntry(
                        "distributed:0:0",
                        "dynamic_event",
                        300f),

                    CreateEntry(
                        "explicit:0",
                        "npc_rescue",
                        750f),

                    CreateEntry(
                        "distributed:0:1",
                        "dynamic_event",
                        900f),

                    CreateEntry(
                        "fixed:0",
                        "boss",
                        1200f),

                    CreateEntry(
                        "distributed:0:2",
                        "dynamic_event",
                        1500f)
                };

            return new TimelineAgenda(
                entries,
                1500f);
        }

        private static TimelineEntry CreateEntry(
            string entryId,
            string eventId,
            float scheduledTime)
        {
            return new TimelineEntry(
                entryId,
                eventId,
                scheduledTime,
                $"icon.{eventId}",
                null);
        }

        private static void TestBeforeFirstEntry(
            TimelineRuntime runtime,
            TimelineAgenda agenda,
            TimelineRuntimeState state)
        {
            TimelineEvaluation evaluation =
                runtime.Evaluate(
                    agenda,
                    TimeSpan.FromSeconds(200),
                    state);

            Assert(
                evaluation.ReachedEntries.Count == 0,
                "Expected no entries before first event.");

            Assert(
                state.NextEntryIndex == 0,
                "NextEntryIndex should remain 0.");

            Assert(
                !evaluation.CompletionTargetReached,
                "Completion target should not be reached.");

            Debug.Log(
                "[Timeline Runtime Test] " +
                "1. Before first entry: PASS");
        }

        private static void TestMultipleEntries(
            TimelineRuntime runtime,
            TimelineAgenda agenda,
            TimelineRuntimeState state)
        {
            TimelineEvaluation evaluation =
                runtime.Evaluate(
                    agenda,
                    TimeSpan.FromSeconds(800),
                    state);

            Assert(
                evaluation.ReachedEntries.Count == 2,
                "Expected two entries at 800 seconds.");

            AssertEntry(
                evaluation.ReachedEntries[0],
                "distributed:0:0",
                300f);

            AssertEntry(
                evaluation.ReachedEntries[1],
                "explicit:0",
                750f);

            Assert(
                state.NextEntryIndex == 2,
                "NextEntryIndex should be 2.");

            Debug.Log(
                "[Timeline Runtime Test] " +
                "2. Multiple entries: PASS");
        }

        private static void TestJumpAcrossMultipleEntries(
            TimelineRuntime runtime,
            TimelineAgenda agenda,
            TimelineRuntimeState state)
        {
            TimelineEvaluation evaluation =
                runtime.Evaluate(
                    agenda,
                    TimeSpan.FromSeconds(1250),
                    state);

            Assert(
                evaluation.ReachedEntries.Count == 2,
                "Expected two new entries at 1250 seconds.");

            AssertEntry(
                evaluation.ReachedEntries[0],
                "distributed:0:1",
                900f);

            AssertEntry(
                evaluation.ReachedEntries[1],
                "fixed:0",
                1200f);

            Assert(
                state.NextEntryIndex == 4,
                "NextEntryIndex should be 4.");

            Debug.Log(
                "[Timeline Runtime Test] " +
                "3. Jump across multiple entries: PASS");
        }

        private static void TestCompletionTarget(
            TimelineRuntime runtime,
            TimelineAgenda agenda,
            TimelineRuntimeState state)
        {
            TimelineEvaluation evaluation =
                runtime.Evaluate(
                    agenda,
                    TimeSpan.FromSeconds(1500),
                    state);

            Assert(
                evaluation.ReachedEntries.Count == 1,
                "Expected the final entry at 1500 seconds.");

            AssertEntry(
                evaluation.ReachedEntries[0],
                "distributed:0:2",
                1500f);

            Assert(
                evaluation.CompletionTargetReached,
                "Completion target should be reached.");

            Assert(
                state.CompletionTargetReached,
                "Runtime state should remember completion.");

            Assert(
                state.NextEntryIndex == 5,
                "All entries should have been processed.");

            Debug.Log(
                "[Timeline Runtime Test] " +
                "4. Completion target: PASS");
        }

        private static void TestAfterCompletionTarget(
            TimelineRuntime runtime,
            TimelineAgenda agenda,
            TimelineRuntimeState state)
        {
            TimelineEvaluation evaluation =
                runtime.Evaluate(
                    agenda,
                    TimeSpan.FromSeconds(1600),
                    state);

            Assert(
                evaluation.ReachedEntries.Count == 0,
                "No new entries should be reached.");

            Assert(
                !evaluation.CompletionTargetReached,
                "Completion target should not be reported twice.");

            Assert(
                state.CompletionTargetReached,
                "Runtime state should remain completed.");

            Assert(
                state.NextEntryIndex == 5,
                "NextEntryIndex should remain 5.");

            Debug.Log(
                "[Timeline Runtime Test] " +
                "5. After completion target: PASS");
        }

        private static void AssertEntry(
            TimelineEntry entry,
            string expectedId,
            float expectedTime)
        {
            Assert(
                entry.EntryId == expectedId,
                $"Expected entry '{expectedId}', " +
                $"got '{entry.EntryId}'.");

            Assert(
                Mathf.Approximately(
                    entry.ScheduledTime,
                    expectedTime),
                $"Expected time {expectedTime}, " +
                $"got {entry.ScheduledTime}.");
        }

        private static void Assert(
            bool condition,
            string message)
        {
            if (!condition)
                throw new Exception(message);
        }
    }
}