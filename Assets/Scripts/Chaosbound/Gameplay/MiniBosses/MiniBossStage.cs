using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using System;

namespace Chaosbound.Gameplay.MiniBosses
{
    /// <summary>
    /// Executes the MiniBoss Domain for the current
    /// Expedition Runtime tick.
    /// </summary>
    public sealed class MiniBossStage :
        IExpeditionRuntimeStage
    {
        private readonly MiniBossDomainDirector
            miniBossDomainDirector;

        public MiniBossStage(
            MiniBossDomainDirector miniBossDomainDirector)
        {
            this.miniBossDomainDirector =
                miniBossDomainDirector
                ?? throw new ArgumentNullException(
                    nameof(miniBossDomainDirector));
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

            miniBossDomainDirector.Execute(
                context);
        }
    }
}