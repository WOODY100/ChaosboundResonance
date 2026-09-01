using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Expeditions.Runtime.Minimap;
using Chaosbound.Content.Expeditions.Runtime.World;
using Chaosbound.Content.Portal.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Config;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Rendering;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Runtime;
using Chaosbound.Gameplay.Navigation;
using Chaosbound.UI.Timeline;
using System;
using UnityEngine;

namespace Chaosbound.Core.Composition
{
    /// <summary>
    /// Coordinates the initialization flow of an expedition.
    /// It does not implement gameplay logic; it only orchestrates
    /// the existing systems of the project.
    /// </summary>
    public sealed class ExpeditionComposition
    {
        private readonly RuntimeExpeditionConfig runtimeConfig;
        private readonly ExpeditionSceneContext sceneContext;
        private readonly BootstrapContext bootstrapContext;

        private MinimapRuntime minimapRuntime;

        public ExpeditionComposition(
                BootstrapContext bootstrapContext,
                RuntimeExpeditionConfig runtimeConfig,
                ExpeditionSceneContext sceneContext)
        {
            this.bootstrapContext = bootstrapContext;
            this.runtimeConfig = runtimeConfig;
            this.sceneContext = sceneContext;
        }

        /// <summary>
        /// Entry point for expedition initialization.
        /// </summary>
        public void Initialize()
        {
            Validate();

            InitializeRuntime();

            InitializeWorld();

            InitializeNavigation();

            InitializeMinimap();

            InitializeGameplay();

            FinalizeInitialization();
        }

        public void Tick()
        {
            RunManager runManager =
                bootstrapContext.RunManager;

            TimelineUI timelineUI =
                sceneContext.TimelineUI;

            if (runManager == null ||
                timelineUI == null)
            {
                return;
            }

            ExpeditionRuntimeState runtimeState =
                runManager.ExpeditionRuntimeState;

            if (runtimeState == null)
            {
                return;
            }

            timelineUI.UpdateProgress(
                (float)runtimeState.ElapsedTime.TotalSeconds);
        }

        #region Validation

        private void Validate()
        {
            if (bootstrapContext == null)
                throw new InvalidOperationException(
                    "BootstrapContext is required before initializing an expedition.");

            if (runtimeConfig == null)
                throw new InvalidOperationException(
                    "RuntimeExpeditionConfig is required before initializing an expedition.");

            if (sceneContext == null)
                throw new InvalidOperationException(
                    "ExpeditionSceneContext is required before initializing an expedition.");
        }

        #endregion

        #region Initialization Stages

        private void InitializeUI()
        {
            PlayerHealth player =
                sceneContext.Player;

            PlayerExperienceSystem xpSystem =
                sceneContext.PlayerExperienceSystem;

            PlayerSkillLoadout loadout =
                sceneContext.PlayerSkillLoadout;

            RunManager runManager =
                bootstrapContext.RunManager;

            LevelUpManager levelUpManager =
                bootstrapContext.LevelUpManager;

            HUDController hud =
                sceneContext.HUDController;

            HUDXPBarUI xpUI =
                sceneContext.HUDXPBarUI;

            HUDLevelUI levelUI =
                sceneContext.HUDLevelUI;

            SkillBarUI skillBar =
                sceneContext.SkillBarUI;

            TimelineUI timelineUI =
                sceneContext.TimelineUI;

            if (hud != null)
                hud.Initialize(
                    player,
                    runManager);

            if (xpUI != null)
                xpUI.Bind(
                    xpSystem);

            if (levelUI != null)
                levelUI.Bind(
                    xpSystem);

            if (skillBar != null)
                skillBar.Initialize(
                    loadout,
                    levelUpManager);

            if (timelineUI != null &&
                runtimeConfig.Timeline != null &&
                runtimeConfig.Timeline.Agenda != null)
            {
                timelineUI.SetAgenda(
                    runtimeConfig.Timeline.Agenda);
            }
        }

        private void InitializeWorld()
        {
            RuntimeWorldConfig world =
                runtimeConfig.World;

            OpenWorldMapGenerator mapGenerator =
                sceneContext.MapGenerator;

            OpenWorldDecorationGenerator decorationGenerator =
                sceneContext.DecorationGenerator;

            if (world == null)
            {
                throw new InvalidOperationException(
                    "RuntimeWorldConfig is missing.");
            }

            if (mapGenerator == null)
            {
                throw new InvalidOperationException(
                    "OpenWorldMapGenerator is missing.");
            }

            if (decorationGenerator == null)
            {
                throw new InvalidOperationException(
                    "OpenWorldDecorationGenerator is missing.");
            }

            mapGenerator.Initialize(
                world);

            mapGenerator.GenerateMap();

            decorationGenerator.Initialize(
                world);

            decorationGenerator.GenerateDecoration();
        }

        private void InitializeMinimap()
        {
            OpenWorldMapGenerator mapGenerator =
                sceneContext.MapGenerator;

            OpenWorldDecorationGenerator decorationGenerator =
                sceneContext.DecorationGenerator;

            MinimapStaticMapView minimapView =
                sceneContext.MinimapStaticMapView;

            MinimapConfig minimapConfig =
                sceneContext.MinimapConfig;

            RuntimeMinimapConfig runtimeMinimapConfig =
                runtimeConfig.Minimap;

            RectTransform minimapMapViewport =
                sceneContext.MinimapMapViewport;

            RectTransform minimapMapContent =
                sceneContext.MinimapMapContent;

            MinimapRuntimeUpdater minimapRuntimeUpdater =
                sceneContext.MinimapRuntimeUpdater;

            MinimapMarkerView playerMarkerView =
                sceneContext.MinimapPlayerMarkerView;

            MinimapMarkerView bossMarkerView =
                sceneContext.MinimapBossMarkerView;

            MinimapMarkerView portalMarkerView =
                sceneContext.MinimapExitPortalMarkerView;

            PlayerHealth player =
                sceneContext.Player;

            RunManager runManager =
                bootstrapContext.RunManager;

            ExpeditionRuntimeState expeditionRuntimeState =
                runManager != null
                    ? runManager.ExpeditionRuntimeState
                    : null;

            ExitPortalData exitPortal =
                runtimeConfig.Completion != null
                    ? runtimeConfig.Completion.ExitPortal
                    : null;

            if (mapGenerator == null)
            {
                throw new InvalidOperationException(
                    "OpenWorldMapGenerator is missing.");
            }

            if (!mapGenerator.IsGenerated)
            {
                throw new InvalidOperationException(
                    "World must be generated before initializing minimap.");
            }

            if (decorationGenerator == null)
            {
                throw new InvalidOperationException(
                    "OpenWorldDecorationGenerator is missing.");
            }

            if (minimapView == null)
            {
                throw new InvalidOperationException(
                    "MinimapStaticMapView is missing.");
            }

            if (minimapConfig == null)
            {
                throw new InvalidOperationException(
                    "MinimapConfig is missing.");
            }

            if (runtimeMinimapConfig == null)
            {
                throw new InvalidOperationException(
                    "RuntimeMinimapConfig is missing.");
            }

            if (minimapMapViewport == null)
            {
                throw new InvalidOperationException(
                    "MinimapMapViewport is missing.");
            }

            if (minimapMapContent == null)
            {
                throw new InvalidOperationException(
                    "MinimapMapContent is missing.");
            }

            if (minimapRuntimeUpdater == null)
            {
                throw new InvalidOperationException(
                    "MinimapRuntimeUpdater is missing.");
            }

            if (playerMarkerView == null)
            {
                throw new InvalidOperationException(
                    "MinimapPlayerMarkerView is missing.");
            }

            if (player == null)
            {
                throw new InvalidOperationException(
                    "Player is missing.");
            }

            if (runManager == null)
            {
                throw new InvalidOperationException(
                    "RunManager is missing.");
            }

            if (expeditionRuntimeState == null)
            {
                throw new InvalidOperationException(
                    "ExpeditionRuntimeState is missing.");
            }

            if (bossMarkerView == null)
            {
                throw new InvalidOperationException(
                    "MinimapBossMarkerView is missing.");
            }

            if (portalMarkerView == null)
            {
                throw new InvalidOperationException(
                    "MinimapExitPortalMarkerView is missing.");
            }

            Bounds worldBounds =
                mapGenerator.GeneratedWorldBounds;

            MinimapOrientationBasis orientationBasis =
                MinimapOrientationBasis.NorthUp;

            MinimapRuntime minimapRuntime =
                new MinimapRuntime(
                    minimapView,
                    minimapConfig,
                    runtimeMinimapConfig,
                    minimapMapViewport,
                    minimapMapContent,
                    orientationBasis);

            //==========================================================
            // 1. Build static map and coordinate system.
            //==========================================================

            minimapRuntime.Build(
                mapGenerator.WorldLayout,
                worldBounds);

            //==========================================================
            // 2. Initialize centered player marker.
            //==========================================================

            minimapRuntime.InitializePlayerMarker(
                player.transform,
                playerMarkerView);

            //==========================================================
            // 3. Initialize Boss marker.
            //==========================================================

            minimapRuntime.InitializeBossMarker(
                expeditionRuntimeState.Boss,
                expeditionRuntimeState.RuntimeReferences,
                bossMarkerView);

            //==========================================================
            // 4. Initialize Exit Portal marker.
            //==========================================================

            minimapRuntime.InitializePortalMarker(
                expeditionRuntimeState.ExitPortal,
                exitPortal,
                expeditionRuntimeState.RuntimeReferences,
                portalMarkerView);

            //==========================================================
            // 5. Initialize fixed world markers.
            //==========================================================

            MinimapMarkerView modifierStructureMarkerView =
                sceneContext.MinimapModifierStructureMarkerView;

            minimapRuntime.InitializeWorldMarkers(
                decorationGenerator.ModifierStructurePositions,
                modifierStructureMarkerView);

            //==========================================================
            // 6. Connect player position to the minimap runtime.
            //==========================================================

            minimapRuntimeUpdater.Initialize(
                minimapRuntime,
                player.transform);

            this.minimapRuntime =
                minimapRuntime;
        }

        private void InitializeNavigation()
        {
            OpenWorldMapGenerator mapGenerator =
                sceneContext.MapGenerator;

            ExpeditionNavigation navigation =
                sceneContext.Navigation;

            if (mapGenerator == null)
            {
                throw new InvalidOperationException(
                    "OpenWorldMapGenerator is missing.");
            }

            if (!mapGenerator.IsGenerated)
            {
                throw new InvalidOperationException(
                    "World must be generated before initializing navigation.");
            }

            if (navigation == null)
            {
                throw new InvalidOperationException(
                    "ExpeditionNavigation is missing.");
            }

            navigation.Initialize(
                mapGenerator.GeneratedWorldBounds);
        }

        private void InitializeRuntime()
        {
            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
            {
                throw new InvalidOperationException(
                    "RunManager is missing.");
            }

            runManager.StartRun(
                runtimeConfig);

            // V1
            // Runtime initialization will be migrated here
            // incrementally in future sprints.
        }

        private void InitializeGameplay()
        {
            InitializeUI();
            InitializeProgression();
            InitializeGameplaySystems();
        }

        private void InitializeGameplaySystems()
        {
            PlayerHealth player =
                sceneContext.Player;

            RunManager runManager =
                bootstrapContext.RunManager;

            EnemyManager enemyManager =
                bootstrapContext.EnemyManager;

            if (player == null)
            {
                throw new InvalidOperationException(
                    "Player is missing.");
            }

            if (runManager == null)
            {
                throw new InvalidOperationException(
                    "RunManager is missing.");
            }

            if (enemyManager == null)
            {
                throw new InvalidOperationException(
                    "EnemyManager is missing.");
            }

            runManager.BindPlayer(
                player);

            enemyManager.SetPlayer(
                player.transform);
        }

        private void InitializeProgression()
        {
            PlayerExperienceSystem xpSystem =
                sceneContext.PlayerExperienceSystem;

            PlayerSkillLoadout loadout =
                sceneContext.PlayerSkillLoadout;

            PlayerStats stats =
                sceneContext.PlayerStats;

            LevelUpManager levelUpManager =
                bootstrapContext.LevelUpManager;

            if (xpSystem == null)
            {
                throw new InvalidOperationException(
                    "PlayerExperienceSystem is missing.");
            }

            if (loadout == null)
            {
                throw new InvalidOperationException(
                    "PlayerSkillLoadout is missing.");
            }

            if (stats == null)
            {
                throw new InvalidOperationException(
                    "PlayerStats is missing.");
            }

            if (levelUpManager == null)
            {
                throw new InvalidOperationException(
                    "LevelUpManager is missing.");
            }

            levelUpManager.Initialize(
                xpSystem,
                loadout,
                stats);
        }

        private void FinalizeInitialization()
        {
            // V1
            // Reserved for future initialization completion logic.
        }

        #endregion
    }
}