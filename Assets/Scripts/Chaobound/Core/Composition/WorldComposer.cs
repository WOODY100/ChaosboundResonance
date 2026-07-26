
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaosbound.Core.Composition
{
    /// <summary>
    /// Executes the world composition pipeline.
    /// </summary>
    public sealed class WorldComposer
    {
        private readonly IReadOnlyList<ICompositionStep> _steps;

        /// <summary>
        /// Creates a new world composer.
        /// </summary>
        /// <param name="steps">
        /// Ordered collection of composition steps.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the steps collection is null.
        /// </exception>
        public WorldComposer(IEnumerable<ICompositionStep> steps)
        {
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));

            _steps = steps.ToList();
        }

        /// <summary>
        /// Executes every composition step using the provided context.
        /// </summary>
        /// <param name="context">
        /// Shared composition context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the context is null.
        /// </exception>
        public void Compose(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            foreach (ICompositionStep step in _steps)
            {
                step.Execute(context);
            }
        }
    }
}