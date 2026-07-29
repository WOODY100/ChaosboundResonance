using Chaosbound.Content.Expeditions.Runtime.Enemy;
using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Produces the candidate set available for tactical evaluation.
    /// </summary>
    public sealed class EnemyQuery
    {
        /// <summary>
        /// Builds the candidate set from the runtime enemy configuration.
        /// </summary>
        public CandidateSet Query(
            RuntimeEnemyConfig enemy,
            TacticalObjective objective)
        {
            if (enemy == null)
            {
                throw new ArgumentNullException(nameof(enemy));
            }

            if (objective == null)
            {
                throw new ArgumentNullException(nameof(objective));
            }

            CandidateSet candidateSet = new();

            foreach (EnemyVariantData variant in enemy.Enemies)
            {
                candidateSet.Add(variant);
            }

            return candidateSet;
        }
    }
}