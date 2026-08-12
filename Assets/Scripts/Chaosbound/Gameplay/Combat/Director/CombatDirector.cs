using Chaosbound.Content.Expeditions.Runtime.Combat;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Gameplay.Combat.Decisions;
using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Gameplay.Combat.Planning;
using Chaosbound.Gameplay.Combat.Results;
using Chaosbound.Gameplay.Combat.Runtime;
using Chaosbound.Gameplay.Combat.Runtime.Replenishment;
using Chaosbound.Gameplay.Combat.Services;
using System;

namespace Chaosbound.Gameplay.Combat.Director
{
    /// <summary>
    /// Coordinates the runtime execution of the Combat Domain.
    ///
    /// The director is a stateless service with respect to
    /// the current expedition. Runtime state is supplied by
    /// the active expedition context.
    /// </summary>
    public sealed class CombatDirector
    {
        private readonly CombatSolver solver;

        private readonly CombatTargetEvaluator targetEvaluator;

        private readonly CombatReconciler reconciler;

        private readonly ReplenishmentController
            replenishmentController;

        private readonly CombatReplenishmentPlanBuilder
            replenishmentPlanBuilder;

        private readonly CombatSpawnPlanner
            spawnPlanner;

        public CombatDirector(
            CombatSolver solver,
            CombatTargetEvaluator targetEvaluator,
            CombatReconciler reconciler,
            ReplenishmentController replenishmentController,
            CombatReplenishmentPlanBuilder replenishmentPlanBuilder,
            CombatSpawnPlanner spawnPlanner)
        {
            this.solver =
                solver
                ?? throw new ArgumentNullException(
                    nameof(solver));

            this.targetEvaluator =
                targetEvaluator
                ?? throw new ArgumentNullException(
                    nameof(targetEvaluator));

            this.reconciler =
                reconciler
                ?? throw new ArgumentNullException(
                    nameof(reconciler));

            this.replenishmentController =
                replenishmentController
                ?? throw new ArgumentNullException(
                    nameof(replenishmentController));

            this.replenishmentPlanBuilder =
                replenishmentPlanBuilder
                ?? throw new ArgumentNullException(
                    nameof(replenishmentPlanBuilder));

            this.spawnPlanner =
                spawnPlanner
                ?? throw new ArgumentNullException(
                    nameof(spawnPlanner));
        }

        /// <summary>
        /// Builds a concrete CombatSpawnPlan from the current
        /// replenishment decision.
        ///
        /// The enemy tier is currently resolved by the
        /// CombatReplenishmentPlanBuilder, which temporarily uses Tier1.
        /// </summary>
        public CombatSpawnPlan BuildSpawnPlan(
            CombatRuntimeState runtimeState,
            RuntimeEnemyConfig enemyConfig,
            ReplenishmentDecision decision)
        {
            if (runtimeState == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeState));
            }

            if (enemyConfig == null)
            {
                throw new ArgumentNullException(
                    nameof(enemyConfig));
            }

            if (!decision.IsRequired)
            {
                runtimeState.SetSpawnPlan(
                    new CombatSpawnPlan(
                        Array.Empty<CombatSpawnPlanEntry>()));

                return runtimeState.SpawnPlan;
            }

            CombatReplenishmentPlan replenishmentPlan =
                replenishmentPlanBuilder.Build(
                    decision);

            CombatSpawnPlan spawnPlan =
                spawnPlanner.Build(
                    replenishmentPlan,
                    enemyConfig);

            runtimeState.SetSpawnPlan(
                spawnPlan);

            return spawnPlan;
        }

        /// <summary>
        /// Sets the active combat tactic for the current
        /// expedition runtime state.
        /// </summary>
        public void SetActiveTactic(
            RuntimeCombatTactic tactic,
            CombatRuntimeState runtimeState)
        {
            if (tactic == null)
            {
                throw new ArgumentNullException(
                    nameof(tactic));
            }

            if (runtimeState == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeState));
            }

            runtimeState.SetActiveTactic(tactic);

            replenishmentController.Reset(
                runtimeState.Replenishment);
        }

        /// <summary>
        /// Resolves the active tactic into a desired
        /// combat composition.
        /// </summary>
        public CombatResult Solve(
            CombatRuntimeState runtimeState,
            RuntimeCombatConfig combatConfig,
            float elapsedSeconds)
        {
            if (runtimeState == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeState));
            }

            if (combatConfig == null)
            {
                throw new ArgumentNullException(
                    nameof(combatConfig));
            }

            RuntimeCombatTactic tactic =
                runtimeState.ActiveTactic;

            if (tactic == null)
            {
                throw new InvalidOperationException(
                    "CombatDirector cannot solve without " +
                    "an active tactic.");
            }

            int target =
                targetEvaluator.Evaluate(
                    combatConfig.TargetProgression,
                    tactic.MaximumTarget,
                    elapsedSeconds);

            CombatResult result =
                solver.Solve(
                    tactic,
                    target);

            runtimeState.SetCombatResult(
                result);

            return result;
        }

        /// <summary>
        /// Reconciles the desired combat composition against
        /// the current materialized population.
        /// </summary>
        public CombatReconciliationResult Reconcile(
            CombatRuntimeState runtimeState,
            CombatPopulationState population)
        {
            if (runtimeState == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeState));
            }

            CombatResult result =
                runtimeState.CombatResult;

            if (result == null)
            {
                throw new InvalidOperationException(
                    "CombatDirector cannot reconcile without " +
                    "a CombatResult.");
            }

            CombatReconciliationResult reconciliation =
                reconciler.Reconcile(
                    result.Composition,
                    population);

            runtimeState.SetReconciliationResult(
                reconciliation);

            return reconciliation;
        }

        /// <summary>
        /// Advances replenishment timing for the current
        /// combat runtime state.
        /// </summary>
        public ReplenishmentDecision TickReplenishment(
            CombatRuntimeState runtimeState,
            float deltaTime)
        {
            if (runtimeState == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeState));
            }

            RuntimeCombatTactic tactic =
                runtimeState.ActiveTactic;

            if (tactic == null)
            {
                throw new InvalidOperationException(
                    "CombatDirector cannot process replenishment " +
                    "without an active tactic.");
            }

            CombatReconciliationResult reconciliation =
                runtimeState.ReconciliationResult;

            if (reconciliation == null)
            {
                throw new InvalidOperationException(
                    "CombatDirector cannot process replenishment " +
                    "without a reconciliation result.");
            }

            return replenishmentController.Tick(
                tactic,
                reconciliation,
                runtimeState.Replenishment,
                deltaTime);
        }

        /// <summary>
        /// Resets the current combat runtime state.
        /// </summary>
        public void Reset(
            CombatRuntimeState runtimeState)
        {
            if (runtimeState == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeState));
            }

            runtimeState.Clear();
        }
    }
}