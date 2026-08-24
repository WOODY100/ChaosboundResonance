using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal
{
    /// <summary>
    /// Executes the Exit Portal Domain for the current
    /// Expedition Runtime tick.
    /// </summary>
    public sealed class ExitPortalStage :
        IExpeditionRuntimeStage
    {
        private readonly ExitPortalDomainDirector
            exitPortalDomainDirector;

        public ExitPortalStage(
            ExitPortalDomainDirector exitPortalDomainDirector)
        {
            this.exitPortalDomainDirector =
                exitPortalDomainDirector
                ?? throw new ArgumentNullException(
                    nameof(exitPortalDomainDirector));
        }

        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            return true;
        }

        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            exitPortalDomainDirector.Execute(
                context);
        }
    }
}