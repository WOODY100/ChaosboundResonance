using Chaosbound.Gameplay.Combat.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Results
{
    /// <summary>
    /// Represents the result of reconciling the desired combat
    /// composition against the current combat population.
    /// </summary>
    public sealed class CombatReconciliationResult
    {
        private readonly List<CombatReconciliationEntry>
            entries;

        /// <summary>
        /// Gets the reconciliation entries.
        /// </summary>
        public IReadOnlyList<CombatReconciliationEntry>
            Entries =>
            entries;

        /// <summary>
        /// Gets whether any role requires replenishment.
        /// </summary>
        public bool RequiresReplenishment
        {
            get
            {
                foreach (
                    CombatReconciliationEntry entry
                    in entries)
                {
                    if (entry.RequiresReplenishment)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the total number of missing enemies.
        /// </summary>
        public int TotalMissingQuantity
        {
            get
            {
                int total = 0;

                foreach (
                    CombatReconciliationEntry entry
                    in entries)
                {
                    total +=
                        entry.MissingQuantity;
                }

                return total;
            }
        }

        public CombatReconciliationResult(
            IReadOnlyList<CombatReconciliationEntry> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(
                    nameof(entries));
            }

            this.entries =
                new List<CombatReconciliationEntry>(
                    entries.Count);

            foreach (
                CombatReconciliationEntry entry
                in entries)
            {
                if (entry == null)
                {
                    throw new InvalidOperationException(
                        "CombatReconciliationResult contains a null entry.");
                }

                this.entries.Add(entry);
            }
        }
    }
}