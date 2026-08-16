using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Tests
{
    /// <summary>
    /// Manual test for the Completion runtime state
    /// integrated into the Expedition Runtime state.
    /// </summary>
    public sealed class CompletionRuntimeStateManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run Completion Runtime State Test")]
        private void RunTest()
        {
            ExpeditionRuntimeState state =
                new ExpeditionRuntimeState();

            // -------------------------------------------------
            // TEST 1
            // Completion runtime state exists.
            // -------------------------------------------------

            if (state.Completion == null)
            {
                throw new Exception(
                    "ExpeditionRuntimeState does not contain " +
                    "a CompletionRuntimeState.");
            }

            Debug.Log(
                "[Completion Runtime State Test] " +
                "PASS 1: Completion runtime state exists.");

            // -------------------------------------------------
            // TEST 2
            // Initial state must be Inactive.
            // -------------------------------------------------

            if (state.Completion.State !=
                CompletionDomainState.Inactive)
            {
                throw new Exception(
                    "Completion Runtime State should start " +
                    "Inactive. " +
                    $"Actual={state.Completion.State}");
            }

            Debug.Log(
                "[Completion Runtime State Test] " +
                "PASS 2: Initial state is Inactive.");

            // -------------------------------------------------
            // TEST 3
            // Expedition Runtime owns the Completion state.
            // -------------------------------------------------

            CompletionRuntimeState firstReference =
                state.Completion;

            CompletionRuntimeState secondReference =
                state.Completion;

            if (!ReferenceEquals(
                firstReference,
                secondReference))
            {
                throw new Exception(
                    "ExpeditionRuntimeState should expose " +
                    "the same CompletionRuntimeState instance.");
            }

            Debug.Log(
                "[Completion Runtime State Test] " +
                "PASS 3: Completion state reference is stable.");

            // -------------------------------------------------
            // FINAL VALIDATION
            // -------------------------------------------------

            Debug.Log(
                "[Completion Runtime State Test] " +
                "ALL TESTS PASSED.");
        }
    }
}