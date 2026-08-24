using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Contracts;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Pipeline
{
    /// <summary>
    /// Executes the configured expedition cleanup stages
    /// in their declared order.
    /// </summary>
    public sealed class ExpeditionCleanupPipeline
    {
        private readonly IReadOnlyList<
            IExpeditionCleanupStage> stages;

        public ExpeditionCleanupPipeline(
            IReadOnlyList<IExpeditionCleanupStage> stages)
        {
            this.stages =
                stages
                ?? throw new ArgumentNullException(
                    nameof(stages));
        }

        /// <summary>
        /// Executes the complete cleanup sequence
        /// for the current expedition.
        /// </summary>
        public void Execute(
            ExpeditionCleanupContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            foreach (
                IExpeditionCleanupStage stage
                in stages)
            {
                if (stage == null)
                    continue;

                stage.Execute(context);
            }
        }
    }
}