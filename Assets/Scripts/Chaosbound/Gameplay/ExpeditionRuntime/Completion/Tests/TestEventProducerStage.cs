using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Tests
{
    /// <summary>
    /// Test-only runtime stage that produces an EventCompleted
    /// during the current pipeline tick.
    /// </summary>
    public sealed class TestEventProducerStage :
        IExpeditionRuntimeStage
    {
        private readonly EventCompleted
            completedEvent;

        private bool hasProduced;

        public TestEventProducerStage(
            EventCompleted completedEvent)
        {
            this.completedEvent =
                completedEvent;
        }

        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            return !hasProduced;
        }

        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            context.State.ReportEventCompleted(
                completedEvent);

            hasProduced = true;
        }
    }
}