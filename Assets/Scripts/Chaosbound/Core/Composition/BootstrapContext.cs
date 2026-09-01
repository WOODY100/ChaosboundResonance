using Chaosbound.Core.GameFlow;
using GameFlowService = Chaosbound.Core.GameFlow.GameFlow;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Config;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Rendering;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Core.Composition
{
    public sealed class BootstrapContext : MonoBehaviour
    {
        public static BootstrapContext Current { get; private set; }

        //==========================================================
        // Persistent Managers
        //==========================================================

        [Header("Persistent Managers")]

        [SerializeField] private RunSession runSession;
        [SerializeField] private RunManager runManager;
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private LevelUpManager levelUpManager;

        //==========================================================
        // Game Flow
        //==========================================================

        [Header("Game Flow")]

        [SerializeField]
        private GameFlowConfiguration gameFlowConfiguration;

        private GameFlowService gameFlow;

        //==========================================================
        // HUD
        //==========================================================

        [Header("HUD")]

        [SerializeField] private HUDController hudController;
        [SerializeField] private HUDXPBarUI hudXPBarUI;
        [SerializeField] private HUDLevelUI hudLevelUI;
        [SerializeField] private SkillBarUI skillBarUI;

        //==========================================================
        // Minimap
        //==========================================================

        [Header("Minimap")]

        [SerializeField]
        private MinimapStaticMapView minimapStaticMapView;

        [SerializeField]
        private MinimapConfig minimapConfig;

        [SerializeField]
        private MinimapRuntimeUpdater minimapRuntimeUpdater;

        [SerializeField]
        private MinimapMarkerView minimapPlayerMarkerView;

        [SerializeField]
        private MinimapMarkerView minimapBossMarkerView;

        [SerializeField]
        private MinimapMarkerView minimapExitPortalMarkerView;

        [SerializeField]
        private MinimapMarkerView minimapModifierStructureMarkerView;

        [SerializeField]
        private RectTransform minimapMapViewport;

        [SerializeField]
        private RectTransform minimapMapContent;

        //==========================================================
        // Private Fields
        //==========================================================

        private SceneTransitionService sceneTransitionService;

        //==========================================================
        // Public Properties
        //==========================================================

        public RunSession RunSession => runSession;
        public RunManager RunManager => runManager;
        public PoolManager PoolManager => poolManager;
        public EnemyManager EnemyManager => enemyManager;
        public LevelUpManager LevelUpManager => levelUpManager;
        public SceneTransitionService SceneTransitionService => sceneTransitionService;

        public GameFlowService GameFlow => gameFlow;

        public HUDController HUDController => hudController;
        public HUDXPBarUI HUDXPBarUI => hudXPBarUI;
        public HUDLevelUI HUDLevelUI => hudLevelUI;
        public SkillBarUI SkillBarUI => skillBarUI;

        public MinimapStaticMapView MinimapStaticMapView =>
            minimapStaticMapView;

        public MinimapConfig MinimapConfig =>
            minimapConfig;

        public MinimapRuntimeUpdater MinimapRuntimeUpdater =>
            minimapRuntimeUpdater;

        public MinimapMarkerView MinimapPlayerMarkerView =>
            minimapPlayerMarkerView;

        public MinimapMarkerView MinimapBossMarkerView =>
            minimapBossMarkerView;

        public MinimapMarkerView MinimapExitPortalMarkerView =>
            minimapExitPortalMarkerView;

        public MinimapMarkerView MinimapModifierStructureMarkerView =>
            minimapModifierStructureMarkerView;

        public RectTransform MinimapMapViewport =>
            minimapMapViewport;

        public RectTransform MinimapMapContent =>
            minimapMapContent;

        //==========================================================
        // Unity
        //==========================================================

        private void Awake()
        {
            RegisterCurrentContext();

            sceneTransitionService =
                new SceneTransitionService();

            CreateGameFlow();
            InitializeGameFlow();
        }

        private void OnDestroy()
        {
            if (Current == this)
            {
                Current = null;
            }
        }

        //==========================================================
        // Initialization
        //==========================================================

        private void RegisterCurrentContext()
        {
            if (Current != null && Current != this)
            {
                throw new InvalidOperationException(
                    "Multiple BootstrapContext instances were detected.");
            }

            Current = this;
        }

        private void CreateGameFlow()
        {
            if (gameFlowConfiguration == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} requires a " +
                    $"{nameof(GameFlowConfiguration)}.");
            }

            GameFlowSimulationController
                simulationController =
                    new GameFlowSimulationController();

            gameFlow =
                new GameFlowService(
                    gameFlowConfiguration,
                    simulationController);
        }

        public void InitializeGameFlow()
        {
            if (gameFlow == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} GameFlow " +
                    "has not been created.");
            }

            gameFlow.Initialize();
        }

#if UNITY_EDITOR

        //==========================================================
        // Validation
        //==========================================================

        private void OnValidate()
        {
            // Persistent Managers
            ValidateReference(
                runSession,
                nameof(runSession));

            ValidateReference(
                runManager,
                nameof(runManager));

            ValidateReference(
                poolManager,
                nameof(poolManager));

            ValidateReference(
                enemyManager,
                nameof(enemyManager));

            ValidateReference(
                levelUpManager,
                nameof(levelUpManager));

            // Game Flow
            ValidateReference(
                gameFlowConfiguration,
                nameof(gameFlowConfiguration));

            // HUD
            ValidateReference(
                hudController,
                nameof(hudController));

            ValidateReference(
                hudXPBarUI,
                nameof(hudXPBarUI));

            ValidateReference(
                hudLevelUI,
                nameof(hudLevelUI));

            ValidateReference(
                skillBarUI,
                nameof(skillBarUI));

            // Minimap
            ValidateReference(
                minimapStaticMapView,
                nameof(minimapStaticMapView));

            ValidateMinimapConfig();

            ValidateReference(
                minimapRuntimeUpdater,
                nameof(minimapRuntimeUpdater));

            ValidateReference(
                minimapPlayerMarkerView,
                nameof(minimapPlayerMarkerView));

            ValidateReference(
                minimapBossMarkerView,
                nameof(minimapBossMarkerView));

            ValidateReference(
                minimapExitPortalMarkerView,
                nameof(minimapExitPortalMarkerView));

            ValidateReference(
                minimapModifierStructureMarkerView,
                nameof(minimapModifierStructureMarkerView));

            ValidateReference(
                minimapMapViewport,
                nameof(minimapMapViewport));

            ValidateReference(
                minimapMapContent,
                nameof(minimapMapContent));
        }

        private void ValidateMinimapConfig()
        {
            if (minimapConfig == null)
            {
                Debug.LogWarning(
                    $"{nameof(BootstrapContext)}: '{nameof(minimapConfig)}' is not assigned.",
                    this);
            }
        }

        private void ValidateReference(
            UnityEngine.Object reference,
            string fieldName)
        {
            if (reference == null)
            {
                Debug.LogWarning(
                    $"{nameof(BootstrapContext)}: '{fieldName}' is not assigned.",
                    this);
            }
        }

#endif
    }
}