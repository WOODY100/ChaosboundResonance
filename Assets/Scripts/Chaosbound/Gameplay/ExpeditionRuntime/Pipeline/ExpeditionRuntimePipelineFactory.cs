using Chaosbound.Gameplay.Combat.Director;
using Chaosbound.Gameplay.Combat.Integration.Spawn;
using Chaosbound.Gameplay.Combat.Runtime.Replenishment;
using Chaosbound.Gameplay.Combat.Services;
using Chaosbound.Gameplay.Combat.Stages;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Providers;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Stages;
using Chaosbound.Gameplay.Pressure.Stages;
using Chaosbound.Gameplay.Spawn.Bootstrap;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Runtime;
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
            IReadOnlyList<IExpeditionRuntimeStage> stages =
                BuildStages();

            return new ExpeditionRuntimePipeline(
                stages);
        }

        private IReadOnlyList<IExpeditionRuntimeStage>
                BuildStages()
            {
                return new List<IExpeditionRuntimeStage>
                    {
                        BuildTimeStage(),
                        BuildPressureStage(),
                        BuildCombatStage()
                    };
            }

        private IExpeditionRuntimeStage
            BuildCombatStage()
        {
            return new CombatStage(
                BuildCombatDirector(),
                BuildCombatSpawnRequestTranslator(),
                BuildSpawnRuntime());
        }

        private IExpeditionRuntimeStage
            BuildTimeStage()
        {
            ITimeProvider timeProvider =
                BuildTimeProvider();

            return new TimeStage(
                timeProvider);
        }

        private IExpeditionRuntimeStage
            BuildPressureStage()
        {
            return new PressureStage();
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
    }
}