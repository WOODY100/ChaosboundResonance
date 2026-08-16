using Chaosbound.Core.Composition;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Tests
{
    /// <summary>
    /// Manual test for validating the Completion runtime configuration
    /// produced as part of the current expedition.
    /// </summary>
    public sealed class CompletionConfigManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run Completion Config Test")]
        private void RunTest()
        {
            BootstrapContext bootstrap =
                BootstrapContext.Current;

            if (bootstrap == null)
            {
                throw new InvalidOperationException(
                    "BootstrapContext is not available.");
            }

            RunSession runSession =
                bootstrap.RunSession;

            if (runSession == null)
            {
                throw new InvalidOperationException(
                    "RunSession is not available.");
            }

            RuntimeExpeditionConfig config =
                runSession.CurrentRun;

            if (config == null)
            {
                throw new InvalidOperationException(
                    "No RuntimeExpeditionConfig is available.");
            }

            // -------------------------------------------------
            // TEST 1
            // Completion runtime configuration exists.
            // -------------------------------------------------

            if (config.Completion == null)
            {
                throw new Exception(
                    "RuntimeExpeditionConfig does not contain " +
                    "a RuntimeCompletionConfig.");
            }

            Debug.Log(
                "[Completion Config Test] " +
                "PASS 1: RuntimeCompletionConfig exists.");

            // -------------------------------------------------
            // TEST 2
            // Completion requirement contains valid data.
            // -------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                config.Completion.Requirement.DomainId))
            {
                throw new Exception(
                    "CompletionRequirement contains an empty DomainId.");
            }

            if (string.IsNullOrWhiteSpace(
                config.Completion.Requirement.EventId))
            {
                throw new Exception(
                    "CompletionRequirement contains an empty EventId.");
            }

            Debug.Log(
                "[Completion Config Test] " +
                "PASS 2: CompletionRequirement contains valid data.");

            // -------------------------------------------------
            // TEST 3
            // DomainId is transported correctly.
            // -------------------------------------------------

            if (config.Completion.Requirement.DomainId !=
                "boss")
            {
                throw new Exception(
                    "Completion DomainId was not transported " +
                    "correctly. " +
                    $"Expected='boss' | " +
                    $"Actual='{config.Completion.Requirement.DomainId}'");
            }

            Debug.Log(
                "[Completion Config Test] " +
                "PASS 3: DomainId transported correctly. " +
                $"DomainId={config.Completion.Requirement.DomainId}");

            // -------------------------------------------------
            // TEST 4
            // EventId is transported correctly.
            // -------------------------------------------------

            if (config.Completion.Requirement.EventId !=
                "boss.minotaur")
            {
                throw new Exception(
                    "Completion EventId was not transported " +
                    "correctly. " +
                    $"Expected='boss.minotaur' | " +
                    $"Actual='{config.Completion.Requirement.EventId}'");
            }

            Debug.Log(
                "[Completion Config Test] " +
                "PASS 4: EventId transported correctly. " +
                $"EventId={config.Completion.Requirement.EventId}");

            // -------------------------------------------------
            // TEST 5
            // Final validation.
            // -------------------------------------------------

            Debug.Log(
                "[Completion Config Test] " +
                "ALL TESTS PASSED.");
        }
    }
}