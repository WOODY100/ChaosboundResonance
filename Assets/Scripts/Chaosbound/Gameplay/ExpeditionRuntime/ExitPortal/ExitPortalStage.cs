using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Pipeline;

namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal
{
    public sealed class ExitPortalStage : IExpeditionRuntimeStage
    {
        private readonly ExitPortalDomainDirector exitPortalDomainDirector;

        public ExitPortalStage(
            ExitPortalDomainDirector exitPortalDomainDirector)
        {
            this.exitPortalDomainDirector =
                exitPortalDomainDirector;
        }

        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            return true;
        }

        public void Execute(
            ExpeditionRuntimeContext context)
        {
            exitPortalDomainDirector.Execute(context);

            if (exitPortalDomainDirector.TryConsumeExitRequest(context))
            {
                context.State.ExitPortal.MarkExitAccepted();
            }
        }
    }
}