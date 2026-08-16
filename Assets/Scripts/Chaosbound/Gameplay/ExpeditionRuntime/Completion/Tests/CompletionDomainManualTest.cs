using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Services;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Factories;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Tests
{
    /// <summary>
    /// Manual test for Completion Domain runtime behavior.
    /// </summary>
    public sealed class CompletionDomainManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run Completion Domain Test")]
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

            CompletionDomainDirector director =
                new CompletionDomainDirector(
                    new CompletionRequirementMatcher());

            // -------------------------------------------------
            // TEST 1
            // Initial state.
            // -------------------------------------------------

            if (context.State.Completion.State !=
                CompletionDomainState.Inactive)
            {
                throw new Exception(
                    "Completion Domain should start Inactive.");
            }

            Debug.Log(
                "[Completion Domain Test] " +
                "PASS 1: Initial state is Inactive.");

            // -------------------------------------------------
            // TEST 2
            // First execution activates Completion.
            // -------------------------------------------------

            director.Execute(
                context);

            if (context.State.Completion.State !=
                CompletionDomainState.Waiting)
            {
                throw new Exception(
                    "Completion Domain did not enter Waiting state.");
            }

            Debug.Log(
                "[Completion Domain Test] " +
                "PASS 2: Domain entered Waiting.");

            // -------------------------------------------------
            // TEST 3
            // Incorrect event must not complete.
            // -------------------------------------------------

            EventCompleted incorrectEvent =
                new EventCompleted(
                    "boss",
                    "boss.other");

            context.State.ReportEventCompleted(
                incorrectEvent);

            director.Execute(
                context);

            if (context.State.EventCompletions.Count != 0)
            {
                throw new Exception(
                    "EventCompletionBuffer was not cleared " +
                    "after Completion processing.");
            }

            if (context.State.Completion.State !=
                CompletionDomainState.Waiting)
            {
                throw new Exception(
                    "Completion Domain completed from " +
                    "an unrelated EventCompleted.");
            }

            Debug.Log(
                "[Completion Domain Test] " +
                "PASS 3: Incorrect event rejected.");

            // -------------------------------------------------
            // TEST 4
            // Correct event completes the expedition.
            // -------------------------------------------------

            EventCompleted correctEvent =
                new EventCompleted(
                    "boss",
                    "boss.minotaur");

            context.State.ReportEventCompleted(
                correctEvent);

            director.Execute(
                context);

            if (context.State.Completion.State !=
                CompletionDomainState.Completed)
            {
                throw new Exception(
                    "Completion Domain did not enter " +
                    "Completed state after matching event.");
            }

            if (context.State.EventCompletions.Count != 0)
            {
                throw new Exception(
                    "EventCompletionBuffer was not cleared " +
                    "after successful Completion.");
            }

            Debug.Log(
                "[Completion Domain Test] " +
                "PASS 4: Matching event completed the Domain.");

            // -------------------------------------------------
            // TEST 5
            // Completed event must be preserved.
            // -------------------------------------------------

            if (!context.State.Completion.CompletedEvent.HasValue)
            {
                throw new Exception(
                    "CompletedEvent was not stored.");
            }

            EventCompleted storedEvent =
                context.State.Completion.CompletedEvent.Value;

            if (storedEvent.DomainId !=
                correctEvent.DomainId ||
                storedEvent.EventId !=
                correctEvent.EventId)
            {
                throw new Exception(
                    "Stored CompletedEvent does not match " +
                    "the event that satisfied the requirement.");
            }

            Debug.Log(
                "[Completion Domain Test] " +
                "PASS 5: CompletedEvent stored correctly.");

            // -------------------------------------------------
            // TEST 6
            // Completed Domain must remain Completed.
            // -------------------------------------------------

            EventCompleted anotherEvent =
                new EventCompleted(
                    "boss",
                    "boss.another");

            context.State.ReportEventCompleted(
                anotherEvent);

            director.Execute(
                context);

            if (context.State.EventCompletions.Count != 0)
            {
                throw new Exception(
                    "EventCompletionBuffer was not cleared " +
                    "after processing a Completed Domain.");
            }

            if (context.State.Completion.State !=
                CompletionDomainState.Completed)
            {
                throw new Exception(
                    "Completion Domain changed state " +
                    "after already being Completed.");
            }

            Debug.Log(
                "[Completion Domain Test] " +
                "PASS 6: Completed state remains terminal.");

            Debug.Log(
                "[Completion Domain Test] " +
                "ALL TESTS PASSED.");
        }
    }
}