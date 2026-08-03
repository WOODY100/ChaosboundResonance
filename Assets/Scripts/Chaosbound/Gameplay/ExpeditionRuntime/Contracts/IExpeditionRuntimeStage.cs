using Chaosbound.Gameplay.ExpeditionRuntime.Context;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Contracts
{
    /// <summary>
    /// Represents a runtime stage executed by the
    /// Expedition Runtime Pipeline.
    /// </summary>
    public interface IExpeditionRuntimeStage
    {
        /// <summary>
        /// Determines whether the stage should execute
        /// during the current runtime tick.
        /// </summary>
        bool ShouldExecute(
            ExpeditionRuntimeContext context);

        /// <summary>
        /// Executes the stage.
        /// </summary>
        void Execute(
            ExpeditionRuntimeContext context);
    }
}