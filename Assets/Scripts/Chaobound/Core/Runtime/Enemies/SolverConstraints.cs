using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the active constraints that limit the search space
    /// of the EnemySolver.
    /// </summary>
    public sealed class SolverConstraints
    {
        /// <summary>
        /// Gets the maximum allowed instances of the same enemy variant.
        /// </summary>
        public int MaxDuplicateVariants { get; }

        /// <summary>
        /// Gets the allowed enemy categories.
        /// </summary>
        public IReadOnlyList<EnemyCategory> AllowedCategories { get; }

        /// <summary>
        /// Gets the roles that must be represented in the final composition.
        /// </summary>
        public IReadOnlyList<EnemyRole> RequiredRoles { get; }

        /// <summary>
        /// Creates a new set of solver constraints.
        /// </summary>
        public SolverConstraints(
            int maxDuplicateVariants,
            IReadOnlyList<EnemyCategory> allowedCategories,
            IReadOnlyList<EnemyRole> requiredRoles)
        {
            if (maxDuplicateVariants < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxDuplicateVariants),
                    "Maximum duplicate variants must be greater than zero.");
            }

            MaxDuplicateVariants = maxDuplicateVariants;

            AllowedCategories = allowedCategories
                ?? throw new ArgumentNullException(nameof(allowedCategories));

            RequiredRoles = requiredRoles
                ?? throw new ArgumentNullException(nameof(requiredRoles));
        }
    }
}