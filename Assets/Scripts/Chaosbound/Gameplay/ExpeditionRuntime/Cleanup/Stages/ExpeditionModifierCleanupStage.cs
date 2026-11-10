using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Stages
{
    /// <summary>
    /// Clears all modifier applications owned by the expedition before
    /// spawned enemies and other runtime objects are cleaned up.
    /// </summary>
    public sealed class ExpeditionModifierCleanupStage :
        IExpeditionCleanupStage
    {
        private readonly ExpeditionModifierDomainDirector director;

        public ExpeditionModifierCleanupStage(
            ExpeditionModifierDomainDirector director)
        {
            this.director = director
                ?? throw new ArgumentNullException(nameof(director));
        }

        public void Execute(ExpeditionCleanupContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            director.Clear(context.RuntimeState.Modifiers);
        }
    }
}
