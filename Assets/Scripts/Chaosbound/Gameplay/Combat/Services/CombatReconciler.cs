using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Gameplay.Combat.Results;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Services
{
    /// <summary>
    /// Reconciles the desired combat composition against
    /// the currently materialized combat population.
    ///
    /// The reconciler does not:
    /// - select enemy variants;
    /// - use RNG;
    /// - create SpawnRequests;
    /// - control Spawn Runtime;
    /// - manage replenishment timing;
    /// - manage Threat Budget;
    /// - calculate Pressure.
    ///
    /// Its only responsibility is to determine the difference
    /// between the desired and current combat states.
    /// </summary>
    public sealed class CombatReconciler
    {
        /// <summary>
        /// Reconciles the desired composition against
        /// the current population.
        /// </summary>
        public CombatReconciliationResult Reconcile(
            CombatComposition desired,
            CombatPopulationState current)
        {
            if (desired == null)
            {
                throw new ArgumentNullException(
                    nameof(desired));
            }

            List<CombatReconciliationEntry>
                entries =
                new List<CombatReconciliationEntry>();

            foreach (
                CombatRuntimeCompositionEntry desiredEntry
                in desired.Entries)
            {
                int currentQuantity =
                    current.GetCount(
                        desiredEntry.CombatType,
                        desiredEntry.Role);

                entries.Add(
                    new CombatReconciliationEntry(
                        desiredEntry.CombatType,
                        desiredEntry.Role,
                        desiredEntry.TargetQuantity,
                        currentQuantity));
            }

            return new CombatReconciliationResult(
                entries);
        }
    }
}