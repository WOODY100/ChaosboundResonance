using Chaosbound.Shared.Enums;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Models
{
    /// <summary>
    /// Represents the desired enemy composition for the
    /// currently selected combat tactic.
    /// </summary>
    public sealed class CombatComposition
    {
        private readonly List<CombatCompositionEntry> entries;

        /// <summary>
        /// Gets the desired composition entries.
        /// </summary>
        public IReadOnlyList<CombatCompositionEntry> Entries =>
            entries;

        /// <summary>
        /// Gets the total number of desired enemies.
        /// </summary>
        public int TotalTargetQuantity
        {
            get
            {
                int total = 0;

                foreach (CombatCompositionEntry entry in entries)
                {
                    total += entry.TargetQuantity;
                }

                return total;
            }
        }

        /// <summary>
        /// Gets whether the composition contains no entries.
        /// </summary>
        public bool IsEmpty =>
            entries.Count == 0;

        /// <summary>
        /// Creates a new immutable combat composition.
        /// </summary>
        public CombatComposition(
            IReadOnlyList<CombatCompositionEntry> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(
                    nameof(entries));
            }

            this.entries =
                new List<CombatCompositionEntry>(
                    entries.Count);

            foreach (CombatCompositionEntry entry in entries)
            {
                if (entry == null)
                {
                    throw new InvalidOperationException(
                        "CombatComposition contains a null entry.");
                }

                if (ContainsRole(entry.Role))
                {
                    throw new InvalidOperationException(
                        $"CombatComposition contains duplicate role '{entry.Role}'.");
                }

                this.entries.Add(entry);
            }
        }

        /// <summary>
        /// Determines whether the composition contains
        /// an entry for the specified role.
        /// </summary>
        public bool ContainsRole(
            EnemyRole role)
        {
            foreach (CombatCompositionEntry entry in entries)
            {
                if (entry.Role == role)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to retrieve the composition entry
        /// associated with the specified role.
        /// </summary>
        public bool TryGetEntry(
            EnemyRole role,
            out CombatCompositionEntry entry)
        {
            foreach (CombatCompositionEntry current in entries)
            {
                if (current.Role == role)
                {
                    entry = current;
                    return true;
                }
            }

            entry = null;
            return false;
        }
    }
}