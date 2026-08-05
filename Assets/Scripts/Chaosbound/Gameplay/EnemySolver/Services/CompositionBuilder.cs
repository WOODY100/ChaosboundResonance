using Chaosbound.Gameplay.EnemySolver.Evaluation;
using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.EnemySolver.ValueObjects;
using Chaosbound.Gameplay.Threat.ValueObjects;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.EnemySolver.Services
{
    /// <summary>
    /// Builds the target enemy composition from a collection of
    /// evaluated candidates.
    /// </summary>
    public sealed class CompositionBuilder
    {
        /// <summary>
        /// Builds the target enemy composition.
        /// </summary>
        public EnemyComposition Build(
            IReadOnlyList<ScoredCandidate> candidates,
            SolverConstraints constraints,
            ThreatCapacity availableThreat)
        {
            ValidateArguments(
                candidates,
                constraints);

            IReadOnlyList<ScoredCandidate> orderedCandidates =
                OrderCandidates(
                    candidates);

            IReadOnlyList<ScoredCandidate> selectedCandidates =
                SelectCandidates(
                    orderedCandidates,
                    constraints,
                    availableThreat);

            IReadOnlyList<EnemyCompositionEntry> entries =
                BuildEntries(
                    selectedCandidates);

            return BuildComposition(
                entries);
        }

        private static void ValidateArguments(
            IReadOnlyList<ScoredCandidate> candidates,
            SolverConstraints constraints)
        {
            if (candidates == null)
                throw new ArgumentNullException(nameof(candidates));

            if (constraints == null)
                throw new ArgumentNullException(nameof(constraints));
        }

        private static IReadOnlyList<ScoredCandidate>
            OrderCandidates(
                IReadOnlyList<ScoredCandidate> candidates)
        {
            List<ScoredCandidate> orderedCandidates =
                new(candidates);

                    orderedCandidates.Sort();

                    orderedCandidates.Reverse();

                    return orderedCandidates;
        }

        private static IReadOnlyList<ScoredCandidate>
            SelectCandidates(
                IReadOnlyList<ScoredCandidate> orderedCandidates,
                SolverConstraints constraints,
                ThreatCapacity availableThreat)
        {
            if (orderedCandidates == null)
                throw new ArgumentNullException(nameof(orderedCandidates));

            if (constraints == null)
                throw new ArgumentNullException(nameof(constraints));

            float remainingThreat =
                availableThreat.Value;

            List<ScoredCandidate> selectedCandidates =
                new();

            foreach (ScoredCandidate candidate in orderedCandidates)
            {
                if (!CanAddCandidate(
                    candidate,
                    constraints,
                    remainingThreat))
                {
                    continue;
                }

                remainingThreat =
                    AddCandidate(
                        candidate,
                        selectedCandidates,
                        remainingThreat);
            }

            return selectedCandidates;
        }

        private static bool HasEnoughThreat(
            ScoredCandidate candidate,
            float remainingThreat)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));

            return candidate.Variant.ThreatCost.Value
                <= remainingThreat;
        }

        private static bool CanAddCandidate(
            ScoredCandidate candidate,
            SolverConstraints constraints,
            float remainingThreat)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));

            if (constraints == null)
                throw new ArgumentNullException(nameof(constraints));

            if (!HasEnoughThreat(
                candidate,
                remainingThreat))
            {
                return false;
            }

            return true;
        }

        private static float AddCandidate(
            ScoredCandidate candidate,
            List<ScoredCandidate> selectedCandidates,
            float remainingThreat)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));

            if (selectedCandidates == null)
                throw new ArgumentNullException(nameof(selectedCandidates));

            selectedCandidates.Add(candidate);

            return remainingThreat
                - candidate.Variant.ThreatCost.Value;
        }

        private static IReadOnlyList<EnemyCompositionEntry>
            BuildEntries(
                IReadOnlyList<ScoredCandidate> candidates)
        {

            List<EnemyCompositionEntry> entries =
                new(candidates.Count);
            foreach (ScoredCandidate candidate in candidates)
            {
                entries.Add(
                    BuildEntry(candidate));
            }

            return entries;
        }

        private static EnemyCompositionEntry
            BuildEntry(
                ScoredCandidate candidate)
        {
            return new EnemyCompositionEntry(
                candidate.Variant,
                1);
        }

        private static EnemyComposition
            BuildComposition(
                IReadOnlyList<EnemyCompositionEntry> entries)
        {
            return new EnemyComposition(entries);
        }
    }
}