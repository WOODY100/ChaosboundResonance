using Chaosbound.Content.Expeditions.Runtime.Combat;
using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Gameplay.Combat.Results;
using Chaosbound.Gameplay.Combat.Runtime.Replenishment;
using System;

namespace Chaosbound.Gameplay.Combat.Runtime
{
    /// <summary>
    /// Represents the persistent runtime state of the Combat Domain.
    ///
    /// This object stores combat state only.
    /// It does not contain configuration or decision-making logic.
    /// </summary>
    public sealed class CombatRuntimeState
    {
        public RuntimeCombatTactic ActiveTactic
        {
            get;
            private set;
        }

        public CombatResult CombatResult
        {
            get;
            private set;
        }

        public CombatReconciliationResult
            ReconciliationResult
        {
            get;
            private set;
        }

        public CombatReplenishmentRuntimeState
            Replenishment
        {
            get;
        }

        public CombatRuntimeState()
        {
            Replenishment =
                new CombatReplenishmentRuntimeState();
        }

        public CombatSpawnPlan SpawnPlan
        {
            get;
            private set;
        }

        public void SetSpawnPlan(
            CombatSpawnPlan spawnPlan)
        {
            SpawnPlan =
                spawnPlan
                ?? throw new ArgumentNullException(
                    nameof(spawnPlan));
        }

        public void SetActiveTactic(
            RuntimeCombatTactic tactic)
        {
            ActiveTactic =
                tactic
                ?? throw new ArgumentNullException(
                    nameof(tactic));
        }

        public void SetCombatResult(
            CombatResult result)
        {
            CombatResult =
                result
                ?? throw new ArgumentNullException(
                    nameof(result));
        }

        public void SetReconciliationResult(
            CombatReconciliationResult result)
        {
            ReconciliationResult =
                result
                ?? throw new ArgumentNullException(
                    nameof(result));
        }

        public void Clear()
        {
            ActiveTactic = null;
            CombatResult = null;
            ReconciliationResult = null;
            SpawnPlan = null;

            Replenishment.Reset();
        }
    }
}