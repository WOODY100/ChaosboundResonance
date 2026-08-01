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
            if (candidates == null)
                throw new ArgumentNullException(nameof(candidates));

            if (constraints == null)
                throw new ArgumentNullException(nameof(constraints));

            List<ScoredCandidate> sortedCandidates =
                new(candidates);

            sortedCandidates.Sort();
            sortedCandidates.Reverse();

            List<EnemyCompositionEntry> entries =
                new();

            foreach (ScoredCandidate candidate in sortedCandidates)
            {
                entries.Add(

                    new EnemyCompositionEntry(

                        candidate.Variant,

                        1));
            }

            return new EnemyComposition(entries);
        }
    }
}