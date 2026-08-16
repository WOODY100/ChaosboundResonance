using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Services;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Factories;
using Chaosbound.Gameplay.ExpeditionRuntime.Pipeline;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Tests
{
    /// <summary>
    /// Validates that an EventCompleted produced by an
    /// earlier runtime stage is consumed by Completion
    /// during the same pipeline tick.
    /// </summary>
    public sealed class CompletionSameTickManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run Completion Same Tick Test")]
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

            CompletionDomainDirector completionDirector =
                new CompletionDomainDirector(
                    new CompletionRequirementMatcher());

            CompletionStage completionStage =
                new CompletionStage(
                    completionDirector);

            EventCompleted completedEvent =
                new EventCompleted(
                    "boss",
                    "boss.minotaur");

            TestEventProducerStage producerStage =
                new TestEventProducerStage(
                    completedEvent);

            IReadOnlyList<IExpeditionRuntimeStage> stages =
                new List<IExpeditionRuntimeStage>
                {
                    producerStage,
                    completionStage
                };

            ExpeditionRuntimePipeline pipeline =
                new ExpeditionRuntimePipeline(
                    stages);

            // -------------------------------------------------
            // TEST 1
            // Completion begins Inactive.
            // -------------------------------------------------

            if (context.State.Completion.State !=
                CompletionDomainState.Inactive)
            {
                throw new Exception(
                    "Completion Domain should start Inactive.");
            }

            Debug.Log(
                "[Completion Same Tick Test] " +
                "PASS 1: Completion starts Inactive.");

            // -------------------------------------------------
            // TEST 2
            // No event exists before the pipeline tick.
            // -------------------------------------------------

            if (context.State.EventCompletions.Count != 0)
            {
                throw new Exception(
                    "EventCompletionBuffer should be empty " +
                    "before the pipeline tick.");
            }

            Debug.Log(
                "[Completion Same Tick Test] " +
                "PASS 2: Buffer starts empty.");

            // -------------------------------------------------
            // TEST 3
            // Execute the pipeline ONCE.
            //
            // Producer creates EventCompleted.
            // Completion consumes it in the same tick.
            // -------------------------------------------------

            pipeline.Execute(
                context);

            if (context.State.Completion.State !=
                CompletionDomainState.Completed)
            {
                throw new Exception(
                    "Completion Domain did not complete " +
                    "during the same pipeline tick.");
            }

            Debug.Log(
                "[Completion Same Tick Test] " +
                "PASS 3: Producer event was consumed " +
                "by Completion in the same tick.");

            // -------------------------------------------------
            // TEST 4
            // Buffer was consumed during the same tick.
            // -------------------------------------------------

            if (context.State.EventCompletions.Count != 0)
            {
                throw new Exception(
                    "EventCompletionBuffer was not cleared " +
                    "after same-tick Completion processing.");
            }

            Debug.Log(
                "[Completion Same Tick Test] " +
                "PASS 4: Buffer was consumed and cleared.");

            // -------------------------------------------------
            // TEST 5
            // CompletedEvent was preserved.
            // -------------------------------------------------

            if (!context.State.Completion.CompletedEvent.HasValue)
            {
                throw new Exception(
                    "CompletedEvent was not preserved.");
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
                    "the event produced during the tick.");
            }

            Debug.Log(
                "[Completion Same Tick Test] " +
                "PASS 5: CompletedEvent preserved correctly.");

            // -------------------------------------------------
            // TEST 6
            // Completion is terminal.
            // -------------------------------------------------

            pipeline.Execute(
                context);

            if (context.State.Completion.State !=
                CompletionDomainState.Completed)
            {
                throw new Exception(
                    "Completion Domain changed state " +
                    "after becoming Completed.");
            }

            Debug.Log(
                "[Completion Same Tick Test] " +
                "PASS 6: Completed state remains terminal.");

            // -------------------------------------------------
            // FINAL VALIDATION
            // -------------------------------------------------

            Debug.Log(
                "[Completion Same Tick Test] " +
                "ALL TESTS PASSED.");
        }
    }
}