using Chaosbound.Shared.Enums;
using Chaosbound.Content.Expeditions.Runtime.Enemy.TacticalIdentity;
using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.EnemySolver.Runtime.Composition;
using Chaosbound.Gameplay.EnemySolver.ValueObjects;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.Threat.ValueObjects;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.EnemySolver.Runtime.Builders
{
    /// <summary>
    /// Builds EnemySolver requests from the current expedition runtime.
    /// </summary>
    public sealed class EnemySolverRequestBuilder
    {
        /// <summary>
        /// Builds a solver request from the current runtime context.
        /// </summary>
        public EnemySolverRequest Build(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return new EnemySolverRequest(
                BuildAvailableEnemies(context),
                BuildPreviousComposition(context),
                BuildRuntimeComposition(context),
                BuildThreatCapacity(context),
                BuildSolverConstraints(context),
                BuildTacticalIdentity(context));
        }

        private IReadOnlyList<EnemyVariantData> BuildAvailableEnemies(
            ExpeditionRuntimeContext context)
        {
            return context.Config.Enemy.Enemies;
        }

        private EnemyComposition BuildPreviousComposition(
            ExpeditionRuntimeContext context)
        {
            EnemySolverResult previousResult =
                context.State.EnemySolverResult;

            if (previousResult == null)
            {
                return new EnemyComposition(
                    Array.Empty<EnemyCompositionEntry>());
            }

            return previousResult.Composition;
        }

        private RuntimeCompositionState BuildRuntimeComposition(
            ExpeditionRuntimeContext context)
        {
            return context.State.RuntimeComposition;
        }

        private ThreatCapacity BuildThreatCapacity(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return new ThreatCapacity(
                context.State
                    .ThreatBudget
                    .AvailableThreat);
        }

        private SolverConstraints BuildSolverConstraints(
            ExpeditionRuntimeContext context)
        {
            return new SolverConstraints(
                int.MaxValue,
                Array.Empty<EnemyCategory>(),
                Array.Empty<EnemyRole>());
        }

        private RuntimeTacticalIdentity BuildTacticalIdentity(
            ExpeditionRuntimeContext context)
        {
            return context.Config.Enemy.TacticalIdentity;
        }
    }
}