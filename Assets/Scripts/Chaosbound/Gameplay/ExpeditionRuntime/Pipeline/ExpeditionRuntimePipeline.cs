using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Pipeline
{
    /// <summary>
    /// Executes the configured Expedition Runtime stages
    /// in their declared order.
    /// </summary>
    public sealed class ExpeditionRuntimePipeline
    {
        private readonly IReadOnlyList<IExpeditionRuntimeStage>
            stages;

        /// <summary>
        /// Creates a new runtime pipeline.
        /// </summary>
        public ExpeditionRuntimePipeline(
            IReadOnlyList<IExpeditionRuntimeStage> stages)
        {
            this.stages =
                stages
                ?? throw new ArgumentNullException(
                    nameof(stages));
        }

        /// <summary>
        /// Executes the configured runtime stages.
        /// </summary>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            foreach (IExpeditionRuntimeStage stage in stages)
            {
                if (!stage.ShouldExecute(context))
                    continue;

                stage.Execute(context);
            }
        }
    }
}