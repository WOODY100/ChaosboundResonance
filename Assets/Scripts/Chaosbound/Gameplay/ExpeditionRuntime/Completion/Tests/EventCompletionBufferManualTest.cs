using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Tests
{
    /// <summary>
    /// Manual test for the EventCompleted runtime buffer.
    /// </summary>
    public sealed class EventCompletionBufferManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run Event Completion Buffer Test")]
        private void RunTest()
        {
            ExpeditionRuntimeState state =
                new ExpeditionRuntimeState();

            // -------------------------------------------------
            // TEST 1
            // Buffer starts empty.
            // -------------------------------------------------

            if (state.EventCompletions == null)
            {
                throw new Exception(
                    "ExpeditionRuntimeState does not contain " +
                    "an EventCompletionBuffer.");
            }

            if (state.EventCompletions.Count != 0)
            {
                throw new Exception(
                    "EventCompletionBuffer should start empty.");
            }

            Debug.Log(
                "[Event Completion Buffer Test] " +
                "PASS 1: Buffer starts empty.");

            // -------------------------------------------------
            // TEST 2
            // Report one completed event.
            // -------------------------------------------------

            EventCompleted bossEvent =
                new EventCompleted(
                    "boss",
                    "boss.minotaur");

            state.ReportEventCompleted(
                bossEvent);

            if (state.EventCompletions.Count != 1)
            {
                throw new Exception(
                    "EventCompletionBuffer should contain " +
                    "one event after reporting.");
            }

            Debug.Log(
                "[Event Completion Buffer Test] " +
                "PASS 2: Event was reported correctly.");

            // -------------------------------------------------
            // TEST 3
            // Stored event must match.
            // -------------------------------------------------

            EventCompleted storedEvent =
                state.EventCompletions.Events[0];

            if (storedEvent.DomainId !=
                bossEvent.DomainId ||
                storedEvent.EventId !=
                bossEvent.EventId)
            {
                throw new Exception(
                    "Stored EventCompleted does not match " +
                    "the reported event.");
            }

            Debug.Log(
                "[Event Completion Buffer Test] " +
                "PASS 3: Stored event matches reported event.");

            // -------------------------------------------------
            // TEST 4
            // Multiple events preserve insertion order.
            // -------------------------------------------------

            EventCompleted npcEvent =
                new EventCompleted(
                    "npc",
                    "npc.prisoner_01");

            EventCompleted chestEvent =
                new EventCompleted(
                    "chest",
                    "chest.ancient_01");

            state.ReportEventCompleted(
                npcEvent);

            state.ReportEventCompleted(
                chestEvent);

            if (state.EventCompletions.Count != 3)
            {
                throw new Exception(
                    "EventCompletionBuffer should contain " +
                    "three events.");
            }

            if (state.EventCompletions.Events[1].EventId !=
                npcEvent.EventId)
            {
                throw new Exception(
                    "EventCompletionBuffer did not preserve " +
                    "insertion order for the NPC event.");
            }

            if (state.EventCompletions.Events[2].EventId !=
                chestEvent.EventId)
            {
                throw new Exception(
                    "EventCompletionBuffer did not preserve " +
                    "insertion order for the Chest event.");
            }

            Debug.Log(
                "[Event Completion Buffer Test] " +
                "PASS 4: Multiple events preserve order.");

            // -------------------------------------------------
            // TEST 5
            // Clear removes all events from the tick.
            // -------------------------------------------------

            state.EventCompletions.Clear();

            if (state.EventCompletions.Count != 0)
            {
                throw new Exception(
                    "EventCompletionBuffer should be empty " +
                    "after Clear().");
            }

            Debug.Log(
                "[Event Completion Buffer Test] " +
                "PASS 5: Buffer clears correctly.");

            // -------------------------------------------------
            // FINAL VALIDATION
            // -------------------------------------------------

            Debug.Log(
                "[Event Completion Buffer Test] " +
                "ALL TESTS PASSED.");
        }
    }
}