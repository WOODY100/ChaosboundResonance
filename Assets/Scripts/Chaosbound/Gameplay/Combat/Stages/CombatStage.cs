using Chaosbound.Content.Expeditions.Runtime.Combat;
using Chaosbound.Gameplay.Combat.Decisions;
using Chaosbound.Gameplay.Combat.Director;
using Chaosbound.Gameplay.Combat.Integration.Spawn;
using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Gameplay.Combat.Results;
using Chaosbound.Gameplay.Combat.Runtime;
using Chaosbound.Gameplay.Combat.Runtime.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;

namespace Chaosbound.Gameplay.Combat.Stages
{
    /// <summary>
    /// Executes the Combat Domain for the current
    /// Expedition Runtime tick.
    /// </summary>
    public sealed class CombatStage :
        IExpeditionRuntimeStage
    {
        private readonly CombatDirector
            combatDirector;

        private readonly CombatSpawnRequestTranslator
            spawnRequestTranslator;

        private readonly SpawnRuntime
            spawnRuntime;

        public CombatStage(
            CombatDirector combatDirector,
            CombatSpawnRequestTranslator spawnRequestTranslator,
            SpawnRuntime spawnRuntime)
        {
            this.combatDirector =
                combatDirector
                ?? throw new ArgumentNullException(
                    nameof(combatDirector));

            this.spawnRequestTranslator =
                spawnRequestTranslator
                ?? throw new ArgumentNullException(
                    nameof(spawnRequestTranslator));

            this.spawnRuntime =
                spawnRuntime
                ?? throw new ArgumentNullException(
                    nameof(spawnRuntime));
        }

        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            RuntimeCombatConfig combatConfig =
                context.Config.Combat;

            if (combatConfig == null)
                return false;

            if (combatConfig.Tactics.Count == 0)
                return false;

            return true;
        }

        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            RuntimeCombatConfig combatConfig =
                context.Config.Combat;

            if (combatConfig == null)
            {
                throw new InvalidOperationException(
                    "CombatStage requires RuntimeCombatConfig.");
            }

            if (combatConfig.Tactics.Count == 0)
            {
                throw new InvalidOperationException(
                    "CombatStage requires at least one CombatTactic.");
            }

            CombatRuntimeState combatState =
                context.State.Combat;

            if (combatState.ActiveTactic == null)
            {
                combatDirector.SetActiveTactic(
                    combatConfig.Tactics[0],
                    combatState);
            }

            combatDirector.Solve(
                combatState,
                combatConfig,
                (float)context.State.ElapsedTime.TotalSeconds);

            CombatPopulationState population =
                CombatPopulationAdapter.Build(
                    context.State.RuntimeComposition);

            CombatReconciliationResult reconciliation =
                combatDirector.Reconcile(
                    combatState,
                    population);

            ReplenishmentDecision replenishmentDecision =
                combatDirector.TickReplenishment(
                    combatState,
                    (float)context.State.DeltaTime.TotalSeconds);

            if (!replenishmentDecision.IsRequired)
            {
                return;
            }

            CombatSpawnPlan spawnPlan =
                combatDirector.BuildSpawnPlan(
                    combatState,
                    context.Config.Enemy,
                    replenishmentDecision);

            SpawnRequest spawnRequest =
                spawnRequestTranslator.Translate(
                    spawnPlan,
                    context.Config.Spawn);

            spawnRuntime.Execute(
                spawnRequest,
                context.Config.Spawn,
                context.References.Runtime,
                context.State);
        }
    }
}