using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.EnemySolver.Evaluation;
using Chaosbound.Gameplay.EnemySolver.ValueObjects;
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
            SolverConstraints constraints)
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
                    constraints);

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
                SolverConstraints constraints)
        {
            return orderedCandidates;
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