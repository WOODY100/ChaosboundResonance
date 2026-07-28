using Chaosbound.Content.Expeditions.Definitions.Population;
using Chaosbound.Shared.Identifiers;
using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Produces the candidate set available for tactical evaluation.
    /// </summary>
    public sealed class PopulationQuery
    {
        /// <summary>
        /// Builds the candidate set from the expedition population.
        /// </summary>
        /// <param name="population">
        /// The expedition population definition.
        /// </param>
        /// <param name="objective">
        /// The tactical objective to satisfy.
        /// </param>
        /// <returns>
        /// The candidate set available for evaluation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when population or objective is null.
        /// </exception>
        public CandidateSet Query(
            PopulationDefinition population,
            TacticalObjective objective)
        {
            if (population == null)
            {
                throw new ArgumentNullException(nameof(population));
            }

            if (objective == null)
            {
                throw new ArgumentNullException(nameof(objective));
            }

            CandidateSet candidateSet = new CandidateSet();

            foreach (ContentReference reference in population.Content)
            {
                candidateSet.Add(reference);
            }

            return candidateSet;
        }
    }
}