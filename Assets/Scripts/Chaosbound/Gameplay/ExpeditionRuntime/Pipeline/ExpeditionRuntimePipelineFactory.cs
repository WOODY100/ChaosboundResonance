using Chaosbound.Gameplay.Bosses;
using Chaosbound.Gameplay.Bosses.Integration.Spawn;
using Chaosbound.Gameplay.Bosses.Services;
using Chaosbound.Gameplay.Combat.Director;
using Chaosbound.Gameplay.Combat.Integration.Spawn;
using Chaosbound.Gameplay.Combat.Runtime.Replenishment;
using Chaosbound.Gameplay.Combat.Services;
using Chaosbound.Gameplay.Combat.Stages;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Services;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Integration.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Services;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Providers;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Stages;
using Chaosbound.Gameplay.MiniBosses;
using Chaosbound.Gameplay.MiniBosses.Integration.Spawn;
using Chaosbound.Gameplay.MiniBosses.Services;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Runtime;
using Chaosbound.Gameplay.Timeline;
using Chaosbound.Gameplay.Timeline.Stages;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers.Stages;
using System;
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
        public ExpeditionRuntimePipeline Create(
            SpawnRuntime spawnRuntime)
        {
            if (spawnRuntime == null)
                throw new ArgumentNullException(
                    nameof(spawnRuntime));

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
                        BuildModifierStage(),
                        BuildTimelineStage(),
                        BuildMiniBossStage(spawnRuntime),
                        BuildBossStage(spawnRuntime),
                        BuildCompletionStage(),
                        BuildExitPortalStage(spawnRuntime),
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

        private IExpeditionRuntimeStage
            BuildModifierStage()
        {
            return new ExpeditionModifierStage(
                new ExpeditionModifierDomainDirector());
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

        private IExpeditionRuntimeStage
            BuildCompletionStage()
        {
            return new CompletionStage(
                new CompletionDomainDirector(
                    new CompletionRequirementMatcher()));
        }

        private IExpeditionRuntimeStage
            BuildExitPortalStage(
                SpawnRuntime spawnRuntime)
        {
            return new ExitPortalStage(
                new ExitPortalDomainDirector(
                    BuildExitPortalSpawnPlanner(),
                    BuildExitPortalSpawnRequestTranslator(),
                    spawnRuntime));
        }

        private ExitPortalSpawnPlanner
            BuildExitPortalSpawnPlanner()
        {
            return new ExitPortalSpawnPlanner();
        }

        private ExitPortalSpawnRequestTranslator
            BuildExitPortalSpawnRequestTranslator()
        {
            SpawnRequestEntryFactory entryFactory =
                new SpawnRequestEntryFactory(
                    new MaterializableReferenceFactory());

            return new ExitPortalSpawnRequestTranslator(
                new SpawnRequestFactory(),
                entryFactory);
        }

        private IExpeditionRuntimeStage
            BuildMiniBossStage(
                SpawnRuntime spawnRuntime)
        {
            return new MiniBossStage(
                new MiniBossDomainDirector(
                    BuildMiniBossSpawnPlanner(),
                    BuildMiniBossSpawnRequestTranslator(),
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

        private MiniBossSpawnPlanner
            BuildMiniBossSpawnPlanner()
        {
            return new MiniBossSpawnPlanner(
                new MiniBossSpawnPlanBuilder());
        }

        private MiniBossSpawnRequestTranslator
            BuildMiniBossSpawnRequestTranslator()
        {
            SpawnRequestEntryFactory entryFactory =
                new SpawnRequestEntryFactory(
                    new MaterializableReferenceFactory());

            return new MiniBossSpawnRequestTranslator(
                new SpawnRequestFactory(),
                entryFactory);
        }
    }
}