using Chaosbound.Gameplay.Bosses;
using Chaosbound.Gameplay.Bosses.Integration.Spawn;
using Chaosbound.Gameplay.Bosses.Services;
using Chaosbound.Gameplay.Combat.Director;
using Chaosbound.Gameplay.Combat.Integration.Spawn;
using Chaosbound.Gameplay.Combat.Runtime.Replenishment;
using Chaosbound.Gameplay.Combat.Services;
using Chaosbound.Gameplay.Combat.Stages;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Providers;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Stages;
using Chaosbound.Gameplay.Spawn.Bootstrap;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Runtime;
using Chaosbound.Gameplay.Timeline;
using Chaosbound.Gameplay.Timeline.Stages;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Pipeline
{
    /// <summary>
    /// Builds the Expedition Runtime Pipeline.
    /// </summary>
    public sealed class ExpeditionRuntimePipelineFactory
    {
        /// <summary>
        /// Creates a new runtime pipeline.
        /// </summary>
        public ExpeditionRuntimePipeline Create()
        {
            SpawnRuntime spawnRuntime =
                BuildSpawnRuntime();

            IReadOnlyList<IExpeditionRuntimeStage> stages =
                BuildStages(
                    spawnRuntime);

            return new ExpeditionRuntimePipeline(
                stages);
        }

        private IReadOnlyList<IExpeditionRuntimeStage>
            BuildStages(
                SpawnRuntime spawnRuntime)
                {
                    return new List<IExpeditionRuntimeStage>
            {
                BuildTimeStage(),
                BuildTimelineStage(),
                BuildBossStage(spawnRuntime),
                BuildCombatStage(spawnRuntime)
            };
        }

        private IExpeditionRuntimeStage
             BuildCombatStage(
        SpawnRuntime spawnRuntime)
        {
            return new CombatStage(
                BuildCombatDirector(),
                BuildCombatSpawnRequestTranslator(),
                spawnRuntime);
        }

        private IExpeditionRuntimeStage
            BuildTimeStage()
        {
            ITimeProvider timeProvider =
                BuildTimeProvider();

            return new TimeStage(
                timeProvider);
        }

        private ITimeProvider
            BuildTimeProvider()
        {
            return new UnityTimeProvider();
        }

        private SpawnRuntime
            BuildSpawnRuntime()
        {
            return new SpawnRuntimeBootstrap()
                .Build();
        }

        private CombatDirector
            BuildCombatDirector()
        {
            return new CombatDirector(
                BuildCombatSolver(),
                BuildCombatTargetEvaluator(),
                BuildCombatReconciler(),
                BuildReplenishmentController(),
                BuildCombatReplenishmentPlanBuilder(),
                BuildCombatSpawnPlanner());
        }

        private CombatReplenishmentPlanBuilder
            BuildCombatReplenishmentPlanBuilder()
        {
            return new CombatReplenishmentPlanBuilder();
        }

        private CombatSpawnRequestTranslator
            BuildCombatSpawnRequestTranslator()
        {
            SpawnRequestEntryFactory entryFactory =
                new SpawnRequestEntryFactory(
                    new MaterializableReferenceFactory());

            return new CombatSpawnRequestTranslator(
                new SpawnRequestFactory(),
                entryFactory);
        }

        private CombatSpawnPlanner
            BuildCombatSpawnPlanner()
        {
            return new CombatSpawnPlanner(
                new EnemyPoolResolver(),
                new EnemyVariantSelector(),
                new CombatSpawnPlanBuilder());
        }

        private CombatSolver
            BuildCombatSolver()
        {
            return new CombatSolver();
        }

        private CombatTargetEvaluator
            BuildCombatTargetEvaluator()
        {
            return new CombatTargetEvaluator();
        }

        private CombatReconciler
            BuildCombatReconciler()
        {
            return new CombatReconciler();
        }

        private ReplenishmentController
            BuildReplenishmentController()
        {
            return new ReplenishmentController();
        }

        private IExpeditionRuntimeStage
            BuildTimelineStage()
        {
            return new TimelineStage(
                new TimelineRuntime());
        }

        private IExpeditionRuntimeStage
             BuildBossStage(
        SpawnRuntime spawnRuntime)
        {
            return new BossStage(
                new BossDomainDirector(
                    BuildBossSpawnPlanner(),
                    BuildBossSpawnRequestTranslator(),
                    spawnRuntime));
        }

        private BossSpawnPlanner
            BuildBossSpawnPlanner()
        {
            return new BossSpawnPlanner();
        }

        private BossSpawnRequestTranslator
            BuildBossSpawnRequestTranslator()
        {
            SpawnRequestEntryFactory entryFactory =
                new SpawnRequestEntryFactory(
                    new MaterializableReferenceFactory());

            return new BossSpawnRequestTranslator(
                new SpawnRequestFactory(),
                entryFactory);
        }
    }
}