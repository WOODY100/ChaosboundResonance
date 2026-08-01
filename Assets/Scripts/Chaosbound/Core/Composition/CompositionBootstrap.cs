using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Composition.Validation;
using System;

namespace Chaosbound.Core.Composition
{
    /// <summary>
    /// Builds and validates the complete expedition composition pipeline.
    /// </summary>
    public sealed class CompositionBootstrap
    {
        private readonly ICompositionContextFactory _contextFactory;
        private readonly WorldComposer _composer;
        private readonly CompositionValidator _validator;

        public CompositionBootstrap(
            ICompositionContextFactory contextFactory,
            WorldComposer composer,
            CompositionValidator validator)
        {
            _contextFactory = contextFactory ??
                throw new ArgumentNullException(nameof(contextFactory));

            _composer = composer ??
                throw new ArgumentNullException(nameof(composer));

            _validator = validator ??
                throw new ArgumentNullException(nameof(validator));
        }

        public CompositionContext Compose(
            RunSession runSession,
            RuntimeExpeditionConfig runtimeConfig)
        {
            if (runSession == null)
                throw new ArgumentNullException(nameof(runSession));

            if (runtimeConfig == null)
                throw new ArgumentNullException(nameof(runtimeConfig));

            CompositionContext context =
                _contextFactory.Create(
                    runSession,
                    runtimeConfig);

            _composer.Compose(context);

            _validator.Validate(context);

            return context;
        }
    }
}