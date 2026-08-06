using System;

namespace Chaosbound.Gameplay.Diagnostics.Threat
{
    /// <summary>
    /// Represents one runtime composition entry
    /// displayed by the diagnostics system.
    /// </summary>
    public sealed class RuntimeCompositionDebugEntry
    {
        public string Name { get; }

        public int AliveCount { get; }

        public RuntimeCompositionDebugEntry(
            string name,
            int aliveCount)
        {
            Name =
                name
                ?? throw new ArgumentNullException(nameof(name));

            AliveCount = aliveCount;
        }
    }
}