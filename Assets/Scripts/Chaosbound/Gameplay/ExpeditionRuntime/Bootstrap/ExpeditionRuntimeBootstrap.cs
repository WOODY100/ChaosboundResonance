using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Gameplay.ExpeditionRuntime.Pipeline;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Bootstrap
{
    /// <summary>
    /// Builds the dependency graph required to execute
    /// the Expedition Runtime.
    /// </summary>
    public sealed class ExpeditionRuntimeBootstrap
    {
        /// <summary>
        /// Builds a fully initialized Expedition Director.
        /// </summary>
        public ExpeditionDirector Build()
        {
            ExpeditionRuntimePipeline pipeline =
                BuildPipeline();

            return new ExpeditionDirector(
                pipeline);
        }

        private ExpeditionRuntimePipeline BuildPipeline()
        {
            ExpeditionRuntimePipelineFactory factory =
                BuildPipelineFactory();

            return factory.Create();
        }

        private ExpeditionRuntimePipelineFactory
            BuildPipelineFactory()
        {
            return new ExpeditionRuntimePipelineFactory();
        }
    }
}