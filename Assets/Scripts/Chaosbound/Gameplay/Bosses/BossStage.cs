using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using System;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Executes the Boss Domain for the current
    /// Expedition Runtime tick.
    /// </summary>
    public sealed class BossStage :
        IExpeditionRuntimeStage
    {
        private readonly BossDomainDirector
            bossDomainDirector;

        public BossStage(
            BossDomainDirector bossDomainDirector)
        {
            this.bossDomainDirector =
                bossDomainDirector
                ?? throw new ArgumentNullException(
                    nameof(bossDomainDirector));
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

            bossDomainDirector.Execute(
                context);
        }
    }
}