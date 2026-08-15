using Chaosbound.Content.Expeditions.Runtime;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Expeditions.Runtime.World;
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
            PlayerHealth player = sceneContext.Player;
            PlayerExperienceSystem xpSystem = sceneContext.PlayerExperienceSystem;

            RunManager runManager = bootstrapContext.RunManager;

            HUDController hud = bootstrapContext.HUDController;
            HUDXPBarUI xpUI = bootstrapContext.HUDXPBarUI;
            HUDLevelUI levelUI = bootstrapContext.HUDLevelUI;

            if (hud != null)
                hud.Initialize(player, runManager);

            if (xpUI != null)
                xpUI.Bind(xpSystem);

            if (levelUI != null)
                levelUI.Bind(xpSystem);
        }

        private void InitializeWorld()
        {
            RuntimeWorldConfig world = runtimeConfig.World;

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

            mapGenerator.Initialize(world);

            mapGenerator.GenerateMap();

            decorationGenerator.Initialize(world);

            decorationGenerator.GenerateDecoration();
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