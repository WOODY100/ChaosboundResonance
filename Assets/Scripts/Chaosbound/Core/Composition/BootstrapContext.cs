using Chaosbound.Core.GameFlow;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Core.Settings;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.UI.Tooltip;
using Chaosbound.Gameplay.MetaProgression.Persistent;
using Chaosbound.Gameplay.Save;
using System;
using UnityEngine;
using GameFlowService = Chaosbound.Core.GameFlow.GameFlow;

namespace Chaosbound.Core.Composition
{
    public sealed class BootstrapContext :
        MonoBehaviour,
        IPersistentStateSaver
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
        [SerializeField] private PersistentInventoryRuntime persistentInventoryRuntime;
        
        private PersistentItemTrashConfirmationService
            persistentItemTrashConfirmationService;

        [SerializeField] private ItemTooltipService itemTooltipService;
        [SerializeField] private GameSettingsRuntime gameSettingsRuntime;
        [SerializeField] private PersistentMetaRuntime persistentMetaRuntime;

        private EquipmentLoadoutRuntime equipmentLoadoutRuntime;
        private SaveGameService saveGameService;
        private EquipmentInventoryService equipmentInventoryService;

        [SerializeField]
        private string saveFileName = "chaosbound_save.json";

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
        public PersistentInventoryRuntime PersistentInventoryRuntime =>
            persistentInventoryRuntime;
        public PersistentItemTrashConfirmationService
            PersistentItemTrashConfirmationService =>
            persistentItemTrashConfirmationService;
        public ItemTooltipService ItemTooltipService =>
            itemTooltipService;
        public GameSettingsRuntime GameSettingsRuntime =>
            gameSettingsRuntime;
        public PersistentMetaRuntime PersistentMetaRuntime =>
            persistentMetaRuntime;
        public EquipmentLoadoutRuntime EquipmentLoadoutRuntime =>
            equipmentLoadoutRuntime;
        public EquipmentInventoryService EquipmentInventoryService =>
            equipmentInventoryService;
        public SaveGameService SaveGameService =>
            saveGameService;

        //==========================================================
        // Unity
        //==========================================================

        private void Awake()
        {
            RegisterCurrentContext();

            equipmentLoadoutRuntime =
                new EquipmentLoadoutRuntime();

            ISaveStorage saveStorage =
                new FileSaveStorage(
                    saveFileName);

            saveGameService =
                new SaveGameService(
                    saveStorage,
                    1);

            sceneTransitionService =
                new SceneTransitionService();

            CreateGameFlow();
            InitializeGameFlow();

            CreatePersistentItemTrashConfirmationService();
        }

        private void Start()
        {
            CreateEquipmentInventoryService();
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

        private void CreatePersistentItemTrashConfirmationService()
        {
            if (persistentInventoryRuntime == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} requires a " +
                    $"{nameof(PersistentInventoryRuntime)}.");
            }

            if (gameFlow == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} GameFlow " +
                    "has not been created.");
            }

            persistentItemTrashConfirmationService =
                new PersistentItemTrashConfirmationService(
                    persistentInventoryRuntime,
                    gameFlow);
        }

        public void SavePersistentState()
        {
            if (saveGameService == null)
                throw new InvalidOperationException(
                    "SaveGameService is not initialized.");

            if (persistentInventoryRuntime == null)
                throw new InvalidOperationException(
                    "PersistentInventoryRuntime is not initialized.");

            if (equipmentLoadoutRuntime == null)
                throw new InvalidOperationException(
                    "EquipmentLoadoutRuntime is not initialized.");

            if (persistentMetaRuntime == null)
                throw new InvalidOperationException(
                    "PersistentMetaRuntime is not initialized.");

            saveGameService.Save(
                persistentInventoryRuntime.State,
                equipmentLoadoutRuntime,
                persistentMetaRuntime.State);
        }

        public bool LoadPersistentState()
        {
            if (saveGameService == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} SaveGameService " +
                    "has not been created.");
            }

            if (persistentInventoryRuntime == null ||
                persistentInventoryRuntime.State == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} PersistentInventoryRuntime " +
                    "is not ready.");
            }

            if (persistentMetaRuntime == null ||
                persistentMetaRuntime.State == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} PersistentMetaRuntime " +
                    "is not ready.");
            }

            if (equipmentLoadoutRuntime == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} EquipmentLoadoutRuntime " +
                    "is not ready.");
            }

            if (!saveGameService.TryLoad(
                    out SaveGameLoadPlan loadPlan))
            {
                return false;
            }

            saveGameService.ApplyLoadPlan(
                loadPlan,
                persistentInventoryRuntime.State,
                equipmentLoadoutRuntime,
                persistentMetaRuntime.State);

            return true;
        }

        private void CreateEquipmentInventoryService()
        {
            if (persistentInventoryRuntime == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} requires a " +
                    $"{nameof(PersistentInventoryRuntime)}.");
            }

            if (equipmentLoadoutRuntime == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} requires a " +
                    $"{nameof(EquipmentLoadoutRuntime)}.");
            }

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} requires a " +
                    $"{nameof(GameContentContext)}.");
            }

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            if (contentResolver == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} requires a valid " +
                    $"{nameof(ItemContentResolver)}.");
            }

            equipmentInventoryService =
                new EquipmentInventoryService(
                    persistentInventoryRuntime,
                    contentResolver,
                    equipmentLoadoutRuntime);
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

            ValidateReference(
                gameFlowConfiguration,
                nameof(gameFlowConfiguration));

            ValidateReference(
                persistentMetaRuntime,
                nameof(persistentMetaRuntime));

            ValidateReference(
                itemTooltipService,
                nameof(itemTooltipService));
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

            if (string.IsNullOrWhiteSpace(saveFileName))
            {
                Debug.LogWarning(
                    $"{nameof(BootstrapContext)}: " +
                    "'saveFileName' is empty. " +
                    "Defaulting to 'chaosbound_save.json'.",
                    this);
            }
        }

#endif
    }
}