using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.EnemySolver.Runtime.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Diagnostics.Threat
{
    /// <summary>
    /// Produces runtime diagnostics for the Threat Budget system.
    /// </summary>
    public sealed class ThreatBudgetDebugger
    {
        private readonly ThreatBudgetDebugFormatter
            formatter;

        public ThreatBudgetDebugger(
            ThreatBudgetDebugFormatter formatter)
        {
            this.formatter =
                formatter
                ?? throw new ArgumentNullException(
                    nameof(formatter));
        }

        /// <summary>
        /// Prints the current runtime state.
        /// </summary>
        public void Print(
            ExpeditionRuntimeState runtimeState,
            EnemySolverResult solverResult)
        {
            Debug.Log(
                $"Debugger Runtime = {runtimeState.GetHashCode()}");

            if (runtimeState == null)
                throw new ArgumentNullException(nameof(runtimeState));

            float investedThreat = 0f;

            List<RuntimeCompositionDebugEntry> composition =
                new();

            int aliveEnemies = 0;

            foreach (RuntimeCompositionEntry entry
                in runtimeState.RuntimeComposition.Entries)
            {
                aliveEnemies +=
                    entry.AliveCount;

                composition.Add(
                    new RuntimeCompositionDebugEntry(
                        entry.Variant.name,
                        entry.AliveCount));
            }

            // TODO:
            // Calculate the total threat currently invested
            // in alive enemies.

            ThreatBudgetDebugSnapshot snapshot =
                new ThreatBudgetDebugSnapshot(
                    (float)runtimeState.ElapsedTime.TotalSeconds,
                    runtimeState.PressureSnapshot.Pressure.Value,
                    runtimeState.ThreatBudget.Capacity.Value,
                    runtimeState.ThreatBudget.UsedThreat,
                    runtimeState.ThreatBudget.AvailableThreat,
                    aliveEnemies,
                    composition);

            Debug.Log(
                formatter.Format(snapshot));
        }
    }
}