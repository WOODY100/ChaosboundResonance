using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Contracts;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Stages
{
    /// <summary>
    /// Cleans up XP fragments materialized during
    /// the current expedition.
    /// </summary>
    public sealed class XPFragmentCleanupStage :
        IExpeditionCleanupStage
    {
        public void Execute(
            ExpeditionCleanupContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            context
                .RuntimeState
                .XPFragments
                .Cleanup();
        }
    }
}