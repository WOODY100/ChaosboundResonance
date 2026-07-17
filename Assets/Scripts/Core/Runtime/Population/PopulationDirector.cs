using Chaosbound.Content.Expeditions.Configs;
using Chaosbound.Content.Expeditions.Definitions;
using Chaosbound.Content.Expeditions.Definitions.Population;
using System;

namespace Chaosbound.Runtime.Population
{
    public sealed class PopulationDirector : IPopulationDirector
    {
        private readonly PopulationConfig _config;

        public PopulationDirector(PopulationConfig config)
        {
            _config = config ??
                throw new ArgumentNullException(nameof(config));

            if (_config.Formations == null)
                throw new ArgumentException(
                    "PopulationConfig must define a Formations collection.",
                    nameof(config));
        }

        public PopulationIntent Evaluate(
            PopulationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (_config.Formations.Count == 0)
                return null;

            PopulationFormation formation = _config.Formations[0];

            return new PopulationIntent(formation);
        }
    }
}