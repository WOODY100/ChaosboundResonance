using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Context;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Contracts
{
    /// <summary>
    /// Represents a cleanup stage executed when
    /// an expedition is being finalized.
    /// </summary>
    public interface IExpeditionCleanupStage
    {
        /// <summary>
        /// Executes cleanup for the current expedition.
        /// </summary>
        void Execute(
            ExpeditionCleanupContext context);
    }
}