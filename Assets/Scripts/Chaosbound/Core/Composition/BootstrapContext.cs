using Chaosbound.Core.GameFlow;
using GameFlowService = Chaosbound.Core.GameFlow.GameFlow;
using Chaosbound.Core.Runtime.SceneManagement;
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