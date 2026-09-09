using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Contracts;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Stages
{
    /// <summary>
    /// Cleans up Loot Bags materialized during
    /// the current expedition.
    /// </summary>
    public sealed class LootBagCleanupStage :
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
                .LootBags
                .Cleanup();
        }
    }
}
