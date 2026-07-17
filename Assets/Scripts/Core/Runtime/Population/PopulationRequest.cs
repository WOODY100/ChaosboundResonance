using Chaosbound.Runtime.Run.Configs.Population;
using Chaosbound.Content.Expeditions.Definitions;
using System;

namespace Chaosbound.Runtime.Population
{
    /// <summary>
    /// Represents a tactical population request produced by the director.
    /// </summary>
    public sealed class PopulationRequest
    {
        public CombatFormation Formation { get; }

        public PopulationRequest(
            CombatFormation formation)
        {
            Formation = formation ??
                throw new ArgumentNullException(nameof(formation));
        }
    }
}