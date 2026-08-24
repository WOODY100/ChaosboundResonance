using Chaosbound.Content.Expeditions.Runtime.Combat;
using Chaosbound.Gameplay.Combat.Decisions;
using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Gameplay.Combat.Results;
using System;

namespace Chaosbound.Gameplay.Combat.Runtime.Replenishment
{
    /// <summary>
    /// Controls the timing of combat replenishment requests.
    ///
    /// The controller does not own runtime state.
    /// All mutable state is stored in the current
    /// CombatReplenishmentRuntimeState.
    /// </summary>
    public sealed class ReplenishmentController
    {
        public ReplenishmentDecision Tick(
            RuntimeCombatTactic tactic,
            CombatReconciliationResult reconciliation,
            CombatReplenishmentRuntimeState state,
            float deltaTime)
        {
            if (tactic == null)
            {
                throw new ArgumentNullException(
                    nameof(tactic));
            }

            if (reconciliation == null)
            {
                throw new ArgumentNullException(
                    nameof(reconciliation));
            }

            if (state == null)
            {
                throw new ArgumentNullException(
                    nameof(state));
            }

            if (deltaTime < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deltaTime),
                    "Delta time cannot be negative.");
            }

            if (!reconciliation.RequiresReplenishment)
            {
                state.Reset();

                return ReplenishmentDecision.None;
            }

            switch (state.CurrentPhase)
            {
                case CombatReplenishmentRuntimeState.Phase.Ready:

                    return BeginInitialDelay(
                        tactic,
                        reconciliation,
                        state,
                        deltaTime);

                case CombatReplenishmentRuntimeState.Phase
                    .WaitingInitialDelay:

                    return ProcessInitialDelay(
                        tactic,
                        reconciliation,
                        state,
                        deltaTime);

                case CombatReplenishmentRuntimeState.Phase
                    .WaitingRecovery:

                    return ProcessRecovery(
                        tactic,
                        reconciliation,
                        state,
                        deltaTime);

                default:

                    throw new InvalidOperationException(
                        $"Unknown replenishment phase " +
                        $"'{state.CurrentPhase}'.");
            }
        }

        public void Reset(
            CombatReplenishmentRuntimeState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(
                    nameof(state));
            }

            state.Reset();
        }

        private ReplenishmentDecision BeginInitialDelay(
            RuntimeCombatTactic tactic,
            CombatReconciliationResult reconciliation,
            CombatReplenishmentRuntimeState state,
            float deltaTime)
        {
            state.SetTimer(0f);

            if (tactic.Replenishment.InitialDelay <= 0f)
            {
                return EmitRequest(
                    reconciliation,
                    state);
            }

            state.SetPhase(
                CombatReplenishmentRuntimeState.Phase
                    .WaitingInitialDelay);

            return ProcessInitialDelay(
                tactic,
                reconciliation,
                state,
                deltaTime);
        }

        private ReplenishmentDecision ProcessInitialDelay(
            RuntimeCombatTactic tactic,
            CombatReconciliationResult reconciliation,
            CombatReplenishmentRuntimeState state,
            float deltaTime)
        {
            state.SetTimer(
                state.Timer + deltaTime);

            if (state.Timer <
                tactic.Replenishment.InitialDelay)
            {
                return ReplenishmentDecision.None;
            }

            return EmitRequest(
                reconciliation,
                state);
        }

        private ReplenishmentDecision ProcessRecovery(
            RuntimeCombatTactic tactic,
            CombatReconciliationResult reconciliation,
            CombatReplenishmentRuntimeState state,
            float deltaTime)
        {
            state.SetTimer(
                state.Timer + deltaTime);

            if (state.Timer <
                tactic.Replenishment.RecoveryInterval)
            {
                return ReplenishmentDecision.None;
            }

            return EmitRequest(
                reconciliation,
                state);
        }

        private ReplenishmentDecision EmitRequest(
            CombatReconciliationResult reconciliation,
            CombatReplenishmentRuntimeState state)
        {
            state.SetTimer(0f);

            state.SetPhase(
                CombatReplenishmentRuntimeState.Phase
                    .WaitingRecovery);

            int entryCount =
                reconciliation.Entries.Count;

            if (entryCount == 0)
            {
                return ReplenishmentDecision.None;
            }

            int startIndex =
                state.NextEntryIndex;

            for (int offset = 0;
                 offset < entryCount;
                 offset++)
            {
                int index =
                    (startIndex + offset) % entryCount;

                CombatReconciliationEntry entry =
                    reconciliation.Entries[index];

                if (!entry.RequiresReplenishment)
                {
                    continue;
                }

                state.SetNextEntryIndex(
                    (index + 1) % entryCount);

                return ReplenishmentDecision.Replenish(
                    entry.CombatType,
                    entry.Role,
                    entry.MissingQuantity);
            }

            return ReplenishmentDecision.None;
        }
    }
}