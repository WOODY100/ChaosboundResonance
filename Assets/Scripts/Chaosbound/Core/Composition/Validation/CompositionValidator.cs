using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaosbound.Core.Composition.Validation
{
    /// <summary>
    /// Executes the composition validation pipeline.
    /// </summary>
    public sealed class CompositionValidator
    {
        private readonly IReadOnlyList<ICompositionValidation> _validations;

        public CompositionValidator(
            IEnumerable<ICompositionValidation> validations)
        {
            if (validations == null)
                throw new ArgumentNullException(nameof(validations));

            _validations = validations.ToList();
        }

        public void Validate(
            CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            foreach (ICompositionValidation validation in _validations)
            {
                validation.Validate(context);
            }
        }
    }
}