using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Runtime.Composition
{
    /// <summary>
    /// Represents the current materialized enemy composition
    /// of the combat runtime.
    /// </summary>
    public sealed class CombatRuntimeComposition
    {
        private readonly List<CombatRuntimeCompositionEntry>
            entries;

        public CombatRuntimeComposition()
        {
            entries =
                new List<CombatRuntimeCompositionEntry>();
        }

        /// <summary>
        /// Gets the currently materialized enemy variants.
        /// </summary>
        public IReadOnlyList<CombatRuntimeCompositionEntry> Entries =>
            entries;

        /// <summary>
        /// Returns whether the specified enemy variant
        /// currently exists in the composition.
        /// </summary>
        public bool Contains(
            EnemyVariantData variant)
        {
            return IndexOf(variant) >= 0;
        }

        /// <summary>
        /// Attempts to retrieve the runtime entry
        /// for the specified enemy variant.
        /// </summary>
        public bool TryGetEntry(
            EnemyVariantData variant,
            out CombatRuntimeCompositionEntry entry)
        {
            int index =
                IndexOf(variant);

            if (index >= 0)
            {
                entry = entries[index];
                return true;
            }

            entry = null;
            return false;
        }

        /// <summary>
        /// Registers one additional alive enemy
        /// of the specified variant.
        /// </summary>
        public void Increment(
            EnemyVariantData variant)
        {
            if (variant == null)
            {
                throw new ArgumentNullException(
                    nameof(variant));
            }

            if (TryGetEntry(
                variant,
                out CombatRuntimeCompositionEntry entry))
            {
                entry.Increment();
                return;
            }

            entries.Add(
                new CombatRuntimeCompositionEntry(
                    variant,
                    1));
        }

        /// <summary>
        /// Removes one alive enemy of the specified variant.
        /// </summary>
        public void Decrement(
            EnemyVariantData variant)
        {
            if (variant == null)
            {
                throw new ArgumentNullException(
                    nameof(variant));
            }

            int index =
                IndexOf(variant);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"Enemy variant '{variant.name}' " +
                    "does not exist in the current runtime composition.");
            }

            CombatRuntimeCompositionEntry entry =
                entries[index];

            entry.Decrement();

            if (entry.AliveCount == 0)
            {
                entries.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clears the current materialized composition.
        /// </summary>
        public void Clear()
        {
            entries.Clear();
        }

        private int IndexOf(
            EnemyVariantData variant)
        {
            if (variant == null)
            {
                throw new ArgumentNullException(
                    nameof(variant));
            }

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Variant == variant)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}