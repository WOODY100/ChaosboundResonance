using System.Collections.Generic;
using Chaosbound.Content.Expeditions.Definitions;

namespace Chaosbound.Content.Expeditions.Configs
{
    public sealed class PopulationConfig
    {
        public IReadOnlyList<EnemyPopulationEntry> Enemies { get; }

        public IReadOnlyList<PopulationFormation> Formations { get; }

        public PopulationConfig(
            IReadOnlyList<EnemyPopulationEntry> enemies,
            IReadOnlyList<PopulationFormation> formations)
        {
            Enemies = enemies;
            Formations = formations;
        }
    }
}