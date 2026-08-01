using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.EnemySolver.Runtime.Composition
{
    /// <summary>
    /// Represents the current materialized composition of the world.
    /// </summary>
    public sealed class RuntimeCompositionState
    {
        private readonly List<RuntimeCompositionEntry> entries;

        public RuntimeCompositionState()
        {
            entries = new List<RuntimeCompositionEntry>();
        }

        /// <summary>
        /// Gets the current runtime composition entries.
        /// </summary>
        public IReadOnlyList<RuntimeCompositionEntry> Entries => entries;

        /// <summary>
        /// Returns whether the specified variant exists.
        /// </summary>
        public bool Contains(EnemyVariantData variant)
        {
            return IndexOf(variant) >= 0;
        }

        /// <summary>
        /// Attempts to retrieve an entry.
        /// </summary>
        public bool TryGetEntry(
            EnemyVariantData variant,
            out RuntimeCompositionEntry entry)
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
        /// Increments the alive count for the specified variant.
        /// </summary>
        public void Increment(EnemyVariantData variant)
        {
            if (variant == null)
                throw new ArgumentNullException(nameof(variant));

            if (TryGetEntry(variant, out RuntimeCompositionEntry entry))
            {
                entry.Increment();
                return;
            }

            entries.Add(
                new RuntimeCompositionEntry(
                    variant,
                    1));
        }

        /// <summary>
        /// Decrements the alive count for the specified variant.
        /// Removes the entry when the count reaches zero.
        /// </summary>
        public void Decrement(EnemyVariantData variant)
        {
            if (variant == null)
                throw new ArgumentNullException(nameof(variant));

            int index = IndexOf(variant);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"Enemy variant '{variant.name}' does not exist in the current composition state.");
            }

            RuntimeCompositionEntry entry = entries[index];

            entry.Decrement();

            if (entry.AliveCount == 0)
            {
                entries.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clears the current runtime composition.
        /// </summary>
        public void Clear()
        {
            entries.Clear();
        }

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