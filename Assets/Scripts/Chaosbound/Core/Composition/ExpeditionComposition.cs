using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Expeditions.Runtime.World;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Config;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Rendering;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Runtime;
using Chaosbound.Gameplay.Navigation;
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

            RunManager runManager =
                bootstrapContext.RunManager;

            HUDController hud =
                bootstrapContext.HUDController;

            HUDXPBarUI xpUI =
                bootstrapContext.HUDXPBarUI;

            HUDLevelUI levelUI =
                bootstrapContext.HUDLevelUI;

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

            MinimapStaticMapView minimapView =
                bootstrapContext.MinimapStaticMapView;

            MinimapConfig minimapConfig =
                bootstrapContext.MinimapConfig;

            RectTransform minimapMapViewport =
                bootstrapContext.MinimapMapViewport;

            RectTransform minimapMapContent =
                bootstrapContext.MinimapMapContent;

            MinimapRuntimeUpdater minimapRuntimeUpdater =
                bootstrapContext.MinimapRuntimeUpdater;

            MinimapMarkerView playerMarkerView =
                bootstrapContext.MinimapPlayerMarkerView;

            PlayerHealth player =
                sceneContext.Player;

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

            Bounds worldBounds =
                mapGenerator.GeneratedWorldBounds;

            MinimapRuntime minimapRuntime =
                new MinimapRuntime(
                    minimapView,
                    minimapConfig,
                    minimapMapViewport,
                    minimapMapContent);

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
            // 3. Connect player position to the minimap runtime.
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
            // V1
            // Reserved for future initialization completion logic.
        }

        private void InitializeProgression()
        {
            // V1
            // Reserved for future initialization completion logic.
        }

        private void FinalizeInitialization()
        {
            // V1
            // Reserved for future initialization completion logic.
        }

        #endregion
    }
}