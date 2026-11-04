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
            bool wasPortalSpawned =
                context.State.ExitPortal.State ==
                Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Runtime.ExitPortalDomainState.Spawned;

            exitPortalDomainDirector.Execute(context);

            bool isPortalSpawned =
                context.State.ExitPortal.State ==
                Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Runtime.ExitPortalDomainState.Spawned;

            if (!wasPortalSpawned && isPortalSpawned)
            {
#if UNITY_EDITOR
                Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
                    .RegisterPhaseStarted(
                        "Extraction",
                        "exit_portal",
                        "Exit Portal",
                        context.State.ElapsedTime.TotalSeconds);
#endif
            }

            if (exitPortalDomainDirector.TryConsumeExitRequest(context))
            {
                context.State.ExitPortal.MarkExitAccepted();

#if UNITY_EDITOR
                Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
                    .RegisterPhaseCompleted(
                        "Extraction",
                        "exit_portal",
                        "Exit Portal",
                        context.State.ElapsedTime.TotalSeconds);
#endif
            }
        }
    }
}