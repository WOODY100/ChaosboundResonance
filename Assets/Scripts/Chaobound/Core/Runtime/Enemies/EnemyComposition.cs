using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the target ecosystem composition that the Enemy Composition Solver
    /// wants to maintain during the current expedition.
    ///
    /// This model is purely declarative and never performs tactical analysis
    /// or decision making.
    /// </summary>
    public sealed class EnemyComposition
    {
        private readonly List<EnemyCompositionEntry> entries;

        public EnemyComposition()
        {
            entries = new List<EnemyCompositionEntry>();
        }

        /// <summary>
        /// Gets the current composition entries.
        /// </summary>
        public IReadOnlyList<EnemyCompositionEntry> Entries => entries;

        /// <summary>
        /// Returns whether the specified variant exists in the composition.
        /// </summary>
        public bool Contains(EnemyVariantData variant)
        {
            return IndexOf(variant) >= 0;
        }

        /// <summary>
        /// Attempts to retrieve the composition entry for the specified variant.
        /// </summary>
        public bool TryGetEntry(
            EnemyVariantData variant,
            out EnemyCompositionEntry entry)
        {
            int index = IndexOf(variant);

            if (index >= 0)
            {
                entry = entries[index];
                return true;
            }

            entry = null;
            return false;
        }

        /// <summary>
        /// Adds a new entry to the composition.
        /// </summary>
        public void Add(EnemyCompositionEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (IndexOf(entry.Variant) >= 0)
            {
                throw new InvalidOperationException(
                    $"Enemy variant '{entry.Variant.name}' already exists in the composition.");
            }

            entries.Add(entry);
        }

        /// <summary>
        /// Removes the specified variant from the composition.
        /// </summary>
        public bool Remove(EnemyVariantData variant)
        {
            int index = IndexOf(variant);

            if (index < 0)
                return false;

            entries.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes every entry from the composition.
        /// </summary>
        public void Clear()
        {
            entries.Clear();
        }

        /// <summary>
        /// Finds the index of the specified variant within the composition.
        /// Returns -1 if the variant is not present.
        /// </summary>
        private int IndexOf(EnemyVariantData variant)
        {
            if (variant == null)
                throw new ArgumentNullException(nameof(variant));

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Variant == variant)
                    return i;
            }

            return -1;
        }
    }
}