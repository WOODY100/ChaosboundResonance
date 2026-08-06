using Chaosbound.Gameplay.EnemySolver.Analysis.Runtime;
using Chaosbound.Gameplay.EnemySolver.Analysis.Services;
using Chaosbound.Gameplay.EnemySolver.Analysis;
using Chaosbound.Gameplay.EnemySolver.Evaluation;
using Chaosbound.Gameplay.EnemySolver.Evaluation.Rules;
using Chaosbound.Gameplay.EnemySolver.Runtime.Builders;
using Chaosbound.Gameplay.EnemySolver.Runtime.Stages;
using Chaosbound.Gameplay.EnemySolver.Services;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Providers;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Stages;
using Chaosbound.Gameplay.Pressure.Stages;
using Chaosbound.Gameplay.Spawn.Bootstrap;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Runtime;
using Chaosbound.Gameplay.Spawn.Stages;
using Chaosbound.Gameplay.Threat.Stages;
using System.Collections.Generic;
using EnemySolverService = Chaosbound.Gameplay.EnemySolver.Services.EnemySolver;

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
                BuildThreatStage(),
                BuildEnemyCompositionStage(),
                BuildSpawnStage()
            };
        }
       
        private IExpeditionRuntimeStage
            BuildEnemyCompositionStage()
        {
            EnemySolverRequestBuilder requestBuilder =
                BuildEnemySolverRequestBuilder();

            EnemySolverService enemySolver =
                BuildEnemySolver();

            return new EnemyCompositionStage(
                requestBuilder,
                enemySolver);
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

        private IExpeditionRuntimeStage
            BuildThreatStage()
        {
            return new ThreatStage();
        }

        private IExpeditionRuntimeStage
            BuildSpawnStage()
        {
            return new SpawnStage(
                BuildSpawnRequestFactory(),
                BuildSpawnRuntime());
        }

        private SpawnRequestFactory
            BuildSpawnRequestFactory()
        {
            return new SpawnRequestFactory();
        }

        private SpawnRuntime
            BuildSpawnRuntime()
        {
            return new SpawnRuntimeBootstrap()
                .Build();
        }

        private EnemySolverRequestBuilder
            BuildEnemySolverRequestBuilder()
        {
            return new EnemySolverRequestBuilder();
        }

        private EnemySolverService
            BuildEnemySolver()
        {
            return new EnemySolverService(
                BuildCandidateBuilder(),
                BuildCandidateValidator(),
                BuildCandidateEvaluator(),
                BuildCompositionBuilder(),
                BuildBudgetAllocator(),
                BuildCompositionAnalyzer());
        }

        private CandidateBuilder
            BuildCandidateBuilder()
        {
            return new CandidateBuilder();
        }

        private CandidateValidator
            BuildCandidateValidator()
        {
            return new CandidateValidator();
        }

        private BudgetAllocator
            BuildBudgetAllocator()
        {
            return new BudgetAllocator();
        }

        private CompositionBuilder
            BuildCompositionBuilder()
        {
            return new CompositionBuilder();
        }

        private CandidateEvaluator
            BuildCandidateEvaluator()
        {
            return new CandidateEvaluator(
                BuildEvaluationRules());
        }

        private IReadOnlyList<IEnemyEvaluationRule>
            BuildEvaluationRules()
        {
            return new IEnemyEvaluationRule[]
            {
                BuildTacticalIdentityRule(),
                BuildNeedCoverageRule()
            };
        }

        private IEnemyEvaluationRule
            BuildTacticalIdentityRule()
        {
            return new TacticalIdentityRule();
        }

        private IEnemyEvaluationRule
            BuildNeedCoverageRule()
        {
            return new NeedCoverageRule();
        }

        private CompositionAnalyzer
            BuildCompositionAnalyzer()
        {
            return new CompositionAnalyzer(
                BuildRuntimeTacticalProfileBuilder(),
                BuildProfileComparator(),
                BuildNeedsAnalyzer(),
                BuildObjectiveSelector());
        }

        private RuntimeTacticalProfileBuilder
            BuildRuntimeTacticalProfileBuilder()
        {
            return new RuntimeTacticalProfileBuilder();
        }

        private ProfileComparator
            BuildProfileComparator()
        {
            return new ProfileComparator();
        }

        private NeedsAnalyzer
            BuildNeedsAnalyzer()
        {
            return new NeedsAnalyzer();
        }

        private ObjectiveSelector
            BuildObjectiveSelector()
        {
            return new ObjectiveSelector();
        }
    }
}