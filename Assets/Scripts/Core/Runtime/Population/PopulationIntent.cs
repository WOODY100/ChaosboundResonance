using System;
using Chaosbound.Content.Expeditions.Definitions;

namespace Chaosbound.Runtime.Population
{
    /// <summary>
    /// Immutable intention produced by the Population Director.
    /// </summary>
    public sealed class PopulationIntent
    {
        public PopulationFormation Formation { get; }

        public PopulationIntent(
            PopulationFormation formation)
        {
            Formation = formation ??
                throw new ArgumentNullException(nameof(formation));
        }
    }
}