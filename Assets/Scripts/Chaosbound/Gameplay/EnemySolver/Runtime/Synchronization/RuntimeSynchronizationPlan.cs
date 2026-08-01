using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the synchronization required to transform the current
    /// runtime composition into the desired composition.
    /// </summary>
    public sealed class RuntimeSynchronizationPlan
    {
        private readonly List<RuntimeSynchronizationEntry> entries;

        public RuntimeSynchronizationPlan()
        {
            entries = new List<RuntimeSynchronizationEntry>();
        }

        /// <summary>
        /// Gets the synchronization entries.
        /// </summary>
        public IReadOnlyList<RuntimeSynchronizationEntry> Entries => entries;

        /// <summary>
        /// Adds a synchronization entry.
        /// </summary>
        public void Add(RuntimeSynchronizationEntry entry)
        {
            if (entry == null)
                throw new System.ArgumentNullException(nameof(entry));

            entries.Add(entry);
        }

        /// <summary>
        /// Returns whether the synchronization contains any pending work.
        /// </summary>
        public bool IsEmpty => entries.Count == 0;

        /// <summary>
        /// Removes all synchronization entries.
        /// </summary>
        public void Clear()
        {
            entries.Clear();
        }
    }
}