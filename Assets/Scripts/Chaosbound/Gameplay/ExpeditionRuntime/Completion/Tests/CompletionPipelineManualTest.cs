using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Factories;
using Chaosbound.Gameplay.ExpeditionRuntime.Pipeline;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Tests
{
    /// <summary>
    /// Manual integration test for Completion
    /// inside the Expedition Runtime Pipeline.
    /// </summary>
    public sealed class CompletionPipelineManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run Completion Pipeline Test")]
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

            if (config.Completion == null)
            {
                throw new InvalidOperationException(
                    "RuntimeCompletionConfig is missing.");
            }

            ExpeditionRuntimeState state =
                new ExpeditionRuntimeState();

            ExpeditionRuntimeContextFactory contextFactory =
                new ExpeditionRuntimeContextFactory(
                    config,
                    state);

            ExpeditionRuntimeContext context =
                contextFactory.Create();

            ExpeditionRuntimePipeline pipeline =
                new ExpeditionRuntimePipelineFactory()
                    .Create();

            // -------------------------------------------------
            // TEST 1
            // Completion starts Inactive.
            // -------------------------------------------------

            if (context.State.Completion.State !=
                CompletionDomainState.Inactive)
            {
                throw new Exception(
                    "Completion Domain should start Inactive.");
            }

            Debug.Log(
                "[Completion Pipeline Test] " +
                "PASS 1: Completion starts Inactive.");

            // -------------------------------------------------
            // TEST 2
            // Report a matching event before the pipeline tick.
            // -------------------------------------------------

            EventCompleted completedEvent =
                new EventCompleted(
                    "boss",
                    "boss.minotaur");

            context.State.ReportEventCompleted(
                completedEvent);

            if (context.State.EventCompletions.Count != 1)
            {
                throw new Exception(
                    "Expected one EventCompleted in the buffer.");
            }

            Debug.Log(
                "[Completion Pipeline Test] " +
                "PASS 2: Matching event entered the runtime buffer.");

            // -------------------------------------------------
            // TEST 3
            // Execute the real Expedition Runtime Pipeline.
            // -------------------------------------------------

            pipeline.Execute(
                context);

            if (context.State.Completion.State !=
                CompletionDomainState.Completed)
            {
                throw new Exception(
                    "Completion Domain did not complete " +
                    "through the Expedition Runtime Pipeline.");
            }

            Debug.Log(
                "[Completion Pipeline Test] " +
                "PASS 3: Pipeline propagated the event to Completion.");

            // -------------------------------------------------
            // TEST 4
            // Buffer must be consumed by Completion.
            // -------------------------------------------------

            if (context.State.EventCompletions.Count != 0)
            {
                throw new Exception(
                    "EventCompletionBuffer was not cleared " +
                    "after Pipeline execution.");
            }

            Debug.Log(
                "[Completion Pipeline Test] " +
                "PASS 4: Completion consumed and cleared the buffer.");

            // -------------------------------------------------
            // TEST 5
            // CompletedEvent must remain stored.
            // -------------------------------------------------

            if (!context.State.Completion.CompletedEvent.HasValue)
            {
                throw new Exception(
                    "CompletedEvent was not stored.");
            }

            EventCompleted storedEvent =
                context.State.Completion.CompletedEvent.Value;

            if (storedEvent.DomainId !=
                completedEvent.DomainId ||
                storedEvent.EventId !=
                completedEvent.EventId)
            {
                throw new Exception(
                    "Stored CompletedEvent does not match " +
                    "the event reported before the Pipeline tick.");
            }

            Debug.Log(
                "[Completion Pipeline Test] " +
                "PASS 5: CompletedEvent preserved correctly.");

            // -------------------------------------------------
            // TEST 6
            // Completion must remain terminal.
            // -------------------------------------------------

            pipeline.Execute(
                context);

            if (context.State.Completion.State !=
                CompletionDomainState.Completed)
            {
                throw new Exception(
                    "Completion Domain changed state after " +
                    "already being Completed.");
            }

            Debug.Log(
                "[Completion Pipeline Test] " +
                "PASS 6: Completed state remains terminal.");

            // -------------------------------------------------
            // FINAL VALIDATION
            // -------------------------------------------------

            Debug.Log(
                "[Completion Pipeline Test] " +
                "ALL TESTS PASSED.");
        }
    }
}