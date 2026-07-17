using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Population
{
    public sealed class PopulationDefinition
    {
        public IReadOnlyList<EnemyPopulationEntry> Enemies { get; }

        public IReadOnlyList<PopulationFormation> Formations { get; }

        public PopulationDefinition(
            IReadOnlyList<EnemyPopulationEntry> enemies,
            IReadOnlyList<PopulationFormation> formations)
        {
            Enemies = enemies;
            Formations = formations;
        }
    }
}