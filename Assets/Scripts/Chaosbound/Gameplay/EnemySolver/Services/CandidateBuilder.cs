using Chaosbound.Gameplay.EnemySolver.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.EnemySolver.Services
{
    /// <summary>
    /// Builds the candidate list consumed by the EnemySolver
    /// from the expedition enemy catalog.
    /// </summary>
    public sealed class CandidateBuilder
    {
        /// <summary>
        /// Builds the solver candidates.
        /// </summary>
        public IReadOnlyList<EnemyCandidate> Build(
            IReadOnlyList<EnemyVariantData> variants)
        {
            if (variants == null)
                throw new ArgumentNullException(nameof(variants));

            List<EnemyCandidate> candidates = new();
            HashSet<EnemyVariantData> uniqueVariants = new();

            foreach (EnemyVariantData variant in variants)
            {
                if (variant == null)
                    continue;

                if (!uniqueVariants.Add(variant))
                    continue;

                candidates.Add(
                    new EnemyCandidate(variant));
            }

            return candidates;
        }
    }
}