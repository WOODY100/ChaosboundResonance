using Chaosbound.Gameplay.EnemySolver.Analysis;
using Chaosbound.Gameplay.EnemySolver.Evaluation;
using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.EnemySolver.ValueObjects;
using Chaosbound.Gameplay.Threat.ValueObjects;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.EnemySolver.Services
{
    /// <summary>
    /// Orchestrates the complete EnemySolver pipeline.
    ///
    /// The EnemySolver contains no tactical intelligence.
    /// Its only responsibility is coordinating the different
    /// stages of the solving pipeline.
    /// </summary>
    public sealed class EnemySolver
    {
        private readonly CandidateBuilder candidateBuilder;
        private readonly CandidateValidator candidateValidator;
        private readonly CandidateEvaluator candidateEvaluator;
        private readonly CompositionBuilder compositionBuilder;
        private readonly BudgetAllocator budgetAllocator;
        private readonly CompositionAnalyzer compositionAnalyzer;

        public EnemySolver(
            CandidateBuilder candidateBuilder,
            CandidateValidator candidateValidator,
            CandidateEvaluator candidateEvaluator,
            CompositionBuilder compositionBuilder,
            BudgetAllocator budgetAllocator,
            CompositionAnalyzer compositionAnalyzer)
        {
            this.candidateBuilder =
                candidateBuilder ?? throw new ArgumentNullException(nameof(candidateBuilder));

            this.candidateValidator =
                candidateValidator ?? throw new ArgumentNullException(nameof(candidateValidator));

            this.candidateEvaluator =
                candidateEvaluator ?? throw new ArgumentNullException(nameof(candidateEvaluator));

            this.compositionBuilder =
                compositionBuilder ?? throw new ArgumentNullException(nameof(compositionBuilder));

            this.budgetAllocator =
                budgetAllocator ?? throw new ArgumentNullException(nameof(budgetAllocator));

            this.compositionAnalyzer =
                compositionAnalyzer ?? throw new ArgumentNullException(nameof(compositionAnalyzer));
        }

        /// <summary>
        /// Solves the current enemy ecosystem.
        /// </summary>
        public EnemySolverResult Solve(
            EnemySolverRequest request)
        {
            ValidateRequest(request);

            IReadOnlyList<EnemyCandidate> candidates =
                candidateBuilder.Build(
                    request.AvailableEnemies);

            IReadOnlyList<EnemyCandidate> validCandidates =
                BuildValidCandidates(
                    candidates,
                    request.Constraints);

            IReadOnlyList<ScoredCandidate> scoredCandidates =
                EvaluateCandidates(
                    validCandidates,
                    request);

            EnemyComposition composition =
                compositionBuilder.Build(
                    scoredCandidates,
                    request.Constraints,
                    request.AvailableThreat);

            SpawnPlan spawnPlan =
                BuildSpawnPlan(
                    composition,
                    request.AvailableThreat);

            return new EnemySolverResult(
                composition,
                spawnPlan);
        }

        /// <summary>
        /// Builds the declarative spawn plan for the generated composition.
        /// </summary>
        private SpawnPlan BuildSpawnPlan(
            EnemyComposition composition,
            ThreatCapacity availableThreat)
        {
            if (composition == null)
                throw new ArgumentNullException(nameof(composition));

            return budgetAllocator.Allocate(
                composition,
                availableThreat);
        }

        /// <summary>
        /// Validates the solver request.
        /// </summary>
        private static void ValidateRequest(
            EnemySolverRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.AvailableEnemies == null)
                throw new ArgumentNullException(nameof(request.AvailableEnemies));

            if (request.PreviousComposition == null)
                throw new ArgumentNullException(nameof(request.PreviousComposition));

            if (request.RuntimeComposition == null)
                throw new ArgumentNullException(nameof(request.RuntimeComposition));

            if (request.Constraints == null)
                throw new ArgumentNullException(nameof(request.Constraints));

            if (request.TacticalIdentity == null)
                throw new ArgumentNullException(nameof(request.TacticalIdentity));
        }

        /// <summary>
        /// Filters the candidates using the current solver constraints.
        /// </summary>
        private IReadOnlyList<EnemyCandidate> BuildValidCandidates(
            IReadOnlyList<EnemyCandidate> candidates,
            SolverConstraints constraints)
        {
            List<EnemyCandidate> validCandidates = new();

            foreach (EnemyCandidate candidate in candidates)
            {
                if (candidateValidator.Validate(candidate, constraints))
                {
                    validCandidates.Add(candidate);
                }
            }

            return validCandidates;
        }

        /// <summary>
        /// Evaluates all valid candidates.
        /// </summary>
        private IReadOnlyList<ScoredCandidate> EvaluateCandidates(
            IReadOnlyList<EnemyCandidate> candidates,
            EnemySolverRequest request)
        {
            List<ScoredCandidate> scoredCandidates = new();

            CompositionAnalysis analysis =
                compositionAnalyzer.Analyze(
                    request.RuntimeComposition);

            EvaluationContext context =
                new EvaluationContext(
                    request.PreviousComposition,
                    new ThreatCost(request.AvailableThreat.Value),
                    request.Constraints,
                    request.TacticalIdentity,
                    analysis);

            foreach (EnemyCandidate candidate in candidates)
            {
                scoredCandidates.Add(
                    candidateEvaluator.Evaluate(
                        candidate,
                        context));
            }

            return scoredCandidates;
        }
    }
}