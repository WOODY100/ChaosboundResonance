using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Stages;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Pipeline
{
    /// <summary>
    /// Builds the Expedition Cleanup Pipeline.
    /// </summary>
    public sealed class ExpeditionCleanupPipelineFactory
    {
        public ExpeditionCleanupPipeline Create(
            SpawnRuntime spawnRuntime)
        {
            if (spawnRuntime == null)
            {
                throw new ArgumentNullException(
                    nameof(spawnRuntime));
            }

            IReadOnlyList<
                IExpeditionCleanupStage> stages =
                BuildStages(
                    spawnRuntime);

            return new ExpeditionCleanupPipeline(
                stages);
        }

        private IReadOnlyList<
            IExpeditionCleanupStage> BuildStages(
                SpawnRuntime spawnRuntime)
        {
            return new List<IExpeditionCleanupStage>
            {
                BuildSpawnCleanupStage(
                    spawnRuntime),
                 
                BuildXPFragmentCleanupStage(),

                BuildSkillCleanupStage()
            };
        }

        private IExpeditionCleanupStage
            BuildSpawnCleanupStage(
                SpawnRuntime spawnRuntime)
        {
            return new SpawnCleanupStage(
                spawnRuntime);
        }

        private IExpeditionCleanupStage
            BuildXPFragmentCleanupStage()
        {
            return new XPFragmentCleanupStage();
        }

        private IExpeditionCleanupStage
            BuildSkillCleanupStage()
        {
            return new SkillCleanupStage();
        }
    }
}