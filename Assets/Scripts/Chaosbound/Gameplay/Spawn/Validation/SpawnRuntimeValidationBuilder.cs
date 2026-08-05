using Chaosbound.Content.Expeditions.Enums.Enemy;
using Chaosbound.Content.Expeditions.Enums.Spawn;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Content.Expeditions.Runtime.Enemy.TacticalIdentity;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Pressure.Models;
using Chaosbound.Gameplay.Pressure.ValueObjects;
using Chaosbound.Gameplay.Spawn.Definitions;
using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Scheduling;
using Chaosbound.Gameplay.Spawn.ValueObjects;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Validation
{
    /// <summary>
    /// Builds a minimal Spawn Runtime validation environment
    /// using the production runtime components.
    /// </summary>
    public sealed class SpawnRuntimeValidationBuilder
    {
        private EnemyVariantData enemy;

        public SpawnRuntimeValidationBuilder WithEnemy(
            EnemyVariantData enemy)
        {
            this.enemy =
                enemy
                ?? throw new ArgumentNullException(nameof(enemy));

            return this;
        }

        public SpawnRuntimeValidationContext Build()
        {
            EnemyVariantData enemy =
                BuildEnemy();

            MaterializableDefinition materializable =
                BuildMaterializable(enemy);

            SpawnExecutionPlanEntry executionEntry =
                BuildExecutionPlanEntry(materializable);

            SpawnJobIdentity identity =
                BuildSpawnJobIdentity();

            SpawnJob job =
                BuildSpawnJob(
                    identity,
                    executionEntry);

            RuntimeEnemyConfig enemyConfig =
                BuildRuntimeEnemyConfig(enemy);

            RuntimeSpawnConfig spawnConfig =
                BuildRuntimeSpawnConfig();

            RuntimeReferencesConfig references =
                BuildRuntimeReferencesConfig();

            PressureSnapshot pressure =
                BuildPressureSnapshot();

            EnemySchedulingContext schedulingContext =
                BuildSchedulingContext(
                    job,
                    enemyConfig,
                    spawnConfig,
                    references,
                    pressure);

            return new SpawnRuntimeValidationContext(
                schedulingContext,
                enemyConfig,
                spawnConfig,
                references,
                pressure);
        }

        private EnemyVariantData BuildEnemy()
        {
            if (enemy == null)
            {
                throw new InvalidOperationException(
                    "Spawn Runtime Validation requires an EnemyVariantData.");
            }

            return enemy;
        }

        private MaterializableDefinition BuildMaterializable(
            EnemyVariantData enemy)
        {
            if (enemy == null)
                throw new ArgumentNullException(nameof(enemy));

            return new MaterializableDefinition(
                enemy);
        }

        private SpawnExecutionPlanEntry BuildExecutionPlanEntry(
            MaterializableDefinition materializable)
        {
            if (materializable == null)
                throw new ArgumentNullException(nameof(materializable));

            return new SpawnExecutionPlanEntry(
                materializable,
                1);
        }

        private RuntimeReferencesConfig BuildRuntimeReferencesConfig()
        {
            GameObject player =
                new GameObject("SpawnRuntimeValidation_Player");

            return new RuntimeReferencesConfig(
                player.transform);
        }

        private SpawnJobIdentity BuildSpawnJobIdentity()
        {
            return SpawnJobIdentity.New();
        }

        private SpawnJob BuildSpawnJob(
            SpawnJobIdentity identity,
            SpawnExecutionPlanEntry executionEntry)
        {
            return new SpawnJob(
                identity,
                executionEntry);
        }

        private RuntimeEnemyConfig BuildRuntimeEnemyConfig(
            EnemyVariantData enemy)
        {
            return new RuntimeEnemyConfig(
                new[]
                {
            enemy
                },
                EnemySchedulingPolicy.Continuous,
                BuildRuntimeTacticalIdentity());
        }

        private RuntimeTacticalIdentity BuildRuntimeTacticalIdentity()
        {
            return new RuntimeTacticalIdentity(
                Array.Empty<RuntimeCapabilityAffinity>());
        }

        private RuntimeSpawnConfig BuildRuntimeSpawnConfig()
        {
            return new RuntimeSpawnConfig(
                SpawnPlacementPolicy.AroundPlayer,
                SpawnActivationPolicy.Immediate,
                Array.Empty<SpawnConstraintPolicy>());
        }

        private PressureSnapshot BuildPressureSnapshot()
        {
            PressureValue pressure =
                new PressureValue(0f);

            return new PressureSnapshot(
                pressure);
        }

        private readonly EnemySchedulingContextFactory
            schedulingContextFactory =
        new EnemySchedulingContextFactory();

        private EnemySchedulingContext BuildSchedulingContext(
            SpawnJob job,
            RuntimeEnemyConfig enemyConfig,
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            PressureSnapshot pressure)
        {
            return schedulingContextFactory.Create(
                job,
                enemyConfig,
                spawnConfig,
                references,
                pressure);
        }
    }
}