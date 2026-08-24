using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers.Stages
{
    /// <summary>
    /// Maintains the runtime state of expedition modifiers.
    /// </summary>
    public sealed class ExpeditionModifierStage :
        IExpeditionRuntimeStage
    {
        private readonly ExpeditionModifierDomainDirector
            director;

        /// <summary>
        /// Creates a new ExpeditionModifierStage.
        /// </summary>
        public ExpeditionModifierStage(
            ExpeditionModifierDomainDirector director)
        {
            this.director =
                director
                ?? throw new ArgumentNullException(
                    nameof(director));
        }

        /// <summary>
        /// Determines whether the modifier stage should
        /// execute during the current runtime tick.
        /// </summary>
        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            return context != null;
        }

        /// <summary>
        /// Executes the modifier domain maintenance.
        /// </summary>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            director.RemoveExpiredModifiers(
                context.State.Modifiers,
                context.State.ElapsedTime);
        }
    }
}