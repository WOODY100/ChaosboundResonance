using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Services;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Factories;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Tests
{
    /// <summary>
    /// Manual test for the Completion runtime pipeline stage.
    /// </summary>
    public sealed class CompletionStageManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run Completion Stage Test")]
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

            CompletionStage stage =
                new CompletionStage(
                    director);

            // -------------------------------------------------
            // TEST 1
            // Stage should execute while Completion is Inactive.
            // -------------------------------------------------

            if (context.State.Completion.State !=
                CompletionDomainState.Inactive)
            {
                throw new Exception(
                    "Completion should initially be Inactive.");
            }

            if (!stage.ShouldExecute(context))
            {
                throw new Exception(
                    "CompletionStage should execute while " +
                    "Completion is Inactive.");
            }

            Debug.Log(
                "[Completion Stage Test] " +
                "PASS 1: Stage executes while Inactive.");

            // -------------------------------------------------
            // TEST 2
            // Stage execution activates Completion.
            // -------------------------------------------------

            stage.Execute(
                context);

            if (context.State.Completion.State !=
                CompletionDomainState.Waiting)
            {
                throw new Exception(
                    "CompletionStage did not activate " +
                    "Completion Domain.");
            }

            Debug.Log(
                "[Completion Stage Test] " +
                "PASS 2: Stage activated Completion.");

            // -------------------------------------------------
            // TEST 3
            // Matching event completes the Domain.
            // -------------------------------------------------

            EventCompleted completedEvent =
                new EventCompleted(
                    "boss",
                    "boss.minotaur");

            context.State.ReportEventCompleted(
                completedEvent);

            stage.Execute(
                context);

            if (context.State.Completion.State !=
                CompletionDomainState.Completed)
            {
                throw new Exception(
                    "CompletionStage did not complete " +
                    "the Completion Domain.");
            }

            Debug.Log(
                "[Completion Stage Test] " +
                "PASS 3: Stage propagated matching event.");

            // -------------------------------------------------
            // TEST 4
            // Completed state stops future execution.
            // -------------------------------------------------

            if (stage.ShouldExecute(context))
            {
                throw new Exception(
                    "CompletionStage should stop executing " +
                    "after Completion becomes Completed.");
            }

            Debug.Log(
                "[Completion Stage Test] " +
                "PASS 4: Stage stops after completion.");

            // -------------------------------------------------
            // FINAL VALIDATION
            // -------------------------------------------------

            Debug.Log(
                "[Completion Stage Test] " +
                "ALL TESTS PASSED.");
        }
    }
}