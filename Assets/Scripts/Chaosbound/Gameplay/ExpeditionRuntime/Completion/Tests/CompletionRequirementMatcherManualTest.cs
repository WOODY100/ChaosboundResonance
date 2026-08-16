using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Services;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Tests
{
    /// <summary>
    /// Manual validation of the Completion Requirement Matcher.
    /// </summary>
    public sealed class CompletionRequirementMatcherManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run Completion Requirement Matcher Test")]
        private void RunTest()
        {
            CompletionRequirementMatcher matcher =
                new CompletionRequirementMatcher();

            CompletionRequirement requirement =
                new CompletionRequirement(
                    "boss",
                    "boss.minotaur");

            // -------------------------------------------------
            // TEST 1
            // Exact match.
            // -------------------------------------------------

            EventCompleted matchingEvent =
                new EventCompleted(
                    "boss",
                    "boss.minotaur");

            if (!matcher.Matches(
                    requirement,
                    matchingEvent))
            {
                throw new Exception(
                    "Matching Boss EventCompleted should satisfy " +
                    "the CompletionRequirement.");
            }

            Debug.Log(
                "[Completion Matcher Test] " +
                "PASS 1: Exact DomainId + EventId match.");

            // -------------------------------------------------
            // TEST 2
            // Same domain, different event.
            // -------------------------------------------------

            EventCompleted differentEvent =
                new EventCompleted(
                    "boss",
                    "boss.other");

            if (matcher.Matches(
                requirement,
                differentEvent))
            {
                throw new Exception(
                    "Different EventId should not satisfy " +
                    "the CompletionRequirement.");
            }

            Debug.Log(
                "[Completion Matcher Test] " +
                "PASS 2: Different EventId rejected.");

            // -------------------------------------------------
            // TEST 3
            // Different domain, same event.
            // -------------------------------------------------

            EventCompleted differentDomain =
                new EventCompleted(
                    "npc",
                    "boss.minotaur");

            if (matcher.Matches(
                requirement,
                differentDomain))
            {
                throw new Exception(
                    "Different DomainId should not satisfy " +
                    "the CompletionRequirement.");
            }

            Debug.Log(
                "[Completion Matcher Test] " +
                "PASS 3: Different DomainId rejected.");

            // -------------------------------------------------
            // TEST 4
            // Completely different event.
            // -------------------------------------------------

            EventCompleted unrelatedEvent =
                new EventCompleted(
                    "chest",
                    "chest.ancient_01");

            if (matcher.Matches(
                requirement,
                unrelatedEvent))
            {
                throw new Exception(
                    "Unrelated EventCompleted should not satisfy " +
                    "the CompletionRequirement.");
            }

            Debug.Log(
                "[Completion Matcher Test] " +
                "PASS 4: Unrelated event rejected.");

            Debug.Log(
                "[Completion Matcher Test] " +
                "ALL TESTS PASSED.");
        }
    }
}