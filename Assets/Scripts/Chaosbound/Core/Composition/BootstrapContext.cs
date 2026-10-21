using Chaosbound.Core.GameFlow;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Core.Settings;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.UI.Tooltip;
using Chaosbound.Gameplay.MetaProgression.Persistent;
using Chaosbound.Gameplay.Save;
using Chaosbound.UI.Loading;
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
        [SerializeField] private SaveFeedbackController saveFeedbackController;

        private PersistentItemTrashConfirmationService
            persistentItemTrashConfirmationService;

        [SerializeField] private ItemTooltipService itemTooltipService;
        [SerializeField] private GameSettingsRuntime gameSettingsRuntime;
        [SerializeField] private VideoSettingsRuntime videoSettingsRuntime;
        [SerializeField] private PersistentMetaRuntime persistentMetaRuntime;

        [SerializeField]
        private SceneTransitionHost sceneTransitionHost;
        [SerializeField]
        private LoadingScreenController loadingScreenController;

        private EquipmentLoadoutRuntime equipmentLoadoutRuntime;
        private ExpeditionProgressRuntime expeditionProgressRuntime;
        private SaveGameService saveGameService;
        private SaveSlotService saveSlotService;
        private EquipmentInventoryService equipmentInventoryService;
        private GameSettingsPersistenceService gameSettingsPersistenceService;
        private VideoSettingsPersistenceService videoSettingsPersistenceService;

        [SerializeField]
        private string settingsFileName = "chaosbound_settings.json";

        [SerializeField]
        private string videoSettingsFileName = "chaosbound_video_settings.json";

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

        private int activeSaveSlotIndex = -1;

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
        public VideoSettingsRuntime VideoSettingsRuntime =>
            videoSettingsRuntime;
        public PersistentMetaRuntime PersistentMetaRuntime =>
            persistentMetaRuntime;
        public EquipmentLoadoutRuntime EquipmentLoadoutRuntime =>
            equipmentLoadoutRuntime;
        public EquipmentInventoryService EquipmentInventoryService =>
            equipmentInventoryService;

        public ExpeditionProgressRuntime ExpeditionProgressRuntime =>
            expeditionProgressRuntime;

        public SaveGameService SaveGameService =>
            saveGameService;

        public SaveSlotService SaveSlotService =>
            saveSlotService;

        public int ActiveSaveSlotIndex =>
            activeSaveSlotIndex;

        public SaveFeedbackController SaveFeedbackController =>
            saveFeedbackController;

        public SceneTransitionHost SceneTransitionHost =>
            sceneTransitionHost;

        public LoadingScreenController LoadingScreenController =>
            loadingScreenController;

        //==========================================================
        // Unity
        //==========================================================

        private void Awake()
        {
            RegisterCurrentContext();

            equipmentLoadoutRuntime =
                new EquipmentLoadoutRuntime();

            expeditionProgressRuntime =
                new ExpeditionProgressRuntime();

            ISaveStorage saveStorage =
                new FileSaveStorage(
                    saveFileName);

            saveGameService =
                new SaveGameService(
                    saveStorage,
                    1);

            saveSlotService =
                new SaveSlotService();

            ISaveStorage settingsStorage =
                new FileSaveStorage(
                    settingsFileName);

            gameSettingsPersistenceService =
                new GameSettingsPersistenceService(
                    settingsStorage);

            if (gameSettingsRuntime == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} requires a " +
                    $"{nameof(GameSettingsRuntime)}.");
            }

            if (videoSettingsRuntime == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} requires a " +
                    $"{nameof(VideoSettingsRuntime)}.");
            }

            ISaveStorage videoSettingsStorage =
                new FileSaveStorage(
                    videoSettingsFileName);

            videoSettingsPersistenceService =
                new VideoSettingsPersistenceService(
                    videoSettingsStorage);

            videoSettingsRuntime.Initialize(
                videoSettingsPersistenceService);

            gameSettingsRuntime.Initialize(
                gameSettingsPersistenceService);

            if (sceneTransitionHost == null)
            {
                throw new InvalidOperationException(
                    "SceneTransitionHost is not assigned.");
            }

            if (loadingScreenController == null)
            {
                throw new InvalidOperationException(
                    "LoadingScreenController is not assigned.");
            }

            sceneTransitionService =
                new SceneTransitionService(
                    sceneTransitionHost,
                    loadingScreenController);

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

        public bool SavePersistentState()
        {
            if (saveSlotService == null)
                throw new InvalidOperationException(
                    "SaveSlotService is not initialized.");

            if (!saveSlotService.IsValidSlot(activeSaveSlotIndex))
            {
                Debug.LogError(
                    "[BootstrapContext] Cannot save because no valid save slot is active.");

                return false;
            }

            if (persistentInventoryRuntime == null)
                throw new InvalidOperationException(
                    "PersistentInventoryRuntime is not initialized.");

            if (equipmentLoadoutRuntime == null)
                throw new InvalidOperationException(
                    "EquipmentLoadoutRuntime is not initialized.");

            if (persistentMetaRuntime == null)
                throw new InvalidOperationException(
                    "PersistentMetaRuntime is not initialized.");

            if (expeditionProgressRuntime == null)
                throw new InvalidOperationException(
                    "ExpeditionProgressRuntime is not initialized.");

            try
            {
                saveSlotService.GetService(activeSaveSlotIndex).Save(
                    persistentInventoryRuntime.State,
                    equipmentLoadoutRuntime,
                    persistentMetaRuntime.State,
                    expeditionProgressRuntime);

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[BootstrapContext] Failed to save persistent state. " +
                    exception);

                return false;
            }
        }

        public bool LoadPersistentState()
        {
            if (!saveSlotService.IsValidSlot(activeSaveSlotIndex))
            {
                Debug.LogWarning(
                    "[BootstrapContext] Cannot load because no valid save slot is active.");

                return false;
            }

            return LoadPersistentState(activeSaveSlotIndex);
        }

        public bool LoadPersistentState(int slotIndex)
        {
            if (saveSlotService == null)
                throw new InvalidOperationException(
                    "SaveSlotService is not initialized.");

            if (!saveSlotService.IsValidSlot(slotIndex))
                return false;

            SaveGameService slotSaveService =
                saveSlotService.GetService(slotIndex);

            if (slotSaveService == null)
            {
                throw new InvalidOperationException(
                    "SaveGameService for the selected slot has not been created.");
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

            if (expeditionProgressRuntime == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(BootstrapContext)} ExpeditionProgressRuntime " +
                    "is not ready.");
            }

            if (!slotSaveService.TryLoad(
                out SaveGameLoadPlan loadPlan))
            {
                return false;
            }


            bool applied = slotSaveService.ApplyLoadPlan(
                loadPlan,
                persistentInventoryRuntime.State,
                equipmentLoadoutRuntime,
                persistentMetaRuntime.State,
                expeditionProgressRuntime);

            if (!applied)
            {
                Debug.LogError(
                    "[BootstrapContext] Failed to apply the save game load plan.");

                return false;
            }

            activeSaveSlotIndex = slotIndex;
            return true;
        }

        public bool ResetPersistentState()
        {
            if (persistentInventoryRuntime == null ||
                persistentInventoryRuntime.State == null)
            {
                Debug.LogError(
                    "[BootstrapContext] PersistentInventoryRuntime is not ready.");
                return false;
            }

            if (persistentMetaRuntime == null ||
                persistentMetaRuntime.State == null)
            {
                Debug.LogError(
                    "[BootstrapContext] PersistentMetaRuntime is not ready.");
                return false;
            }

            if (equipmentLoadoutRuntime == null)
            {
                Debug.LogError(
                    "[BootstrapContext] EquipmentLoadoutRuntime is not ready.");
                return false;
            }

            if (expeditionProgressRuntime == null)
            {
                Debug.LogError(
                    "[BootstrapContext] ExpeditionProgressRuntime is not ready.");
                return false;
            }

            persistentInventoryRuntime.State.Clear();

            equipmentLoadoutRuntime.Clear();

            persistentMetaRuntime.ResetProgression();

            expeditionProgressRuntime.Clear();

            return true;
        }

        public bool TryStartNewGame(int slotIndex)
        {
            if (saveSlotService == null)
            {
                Debug.LogError(
                    "[BootstrapContext] SaveSlotService is not initialized.");
                return false;
            }

            if (!saveSlotService.IsValidSlot(slotIndex))
            {
                Debug.LogWarning(
                    $"[BootstrapContext] Invalid save slot index: {slotIndex}.");
                return false;
            }

            if (saveSlotService.GetSlotStatus(slotIndex) !=
                SaveSlotStatus.Empty)
            {
                Debug.LogWarning(
                    "[BootstrapContext] Cannot start a new game " +
                    "because the selected slot is not empty.");
                return false;
            }

            if (!ResetPersistentState())
                return false;

            activeSaveSlotIndex = slotIndex;

            // Create the initial save immediately.
            if (!SavePersistentState())
            {
                Debug.LogError(
                    "[BootstrapContext] Failed to create the initial save " +
                    $"for slot {slotIndex + 1}.");

                activeSaveSlotIndex = -1;
                return false;
            }

            return true;
        }

        public bool TryLoadGameFromSlot(int slotIndex)
        {
            if (saveSlotService == null)
            {
                Debug.LogError(
                    "[BootstrapContext] SaveSlotService is not initialized.");
                return false;
            }

            if (!saveSlotService.IsValidSlot(slotIndex))
            {
                Debug.LogWarning(
                    $"[BootstrapContext] Invalid save slot index: {slotIndex}.");
                return false;
            }

            if (saveSlotService.GetSlotStatus(slotIndex) !=
                SaveSlotStatus.Available)
            {
                Debug.LogWarning(
                    "[BootstrapContext] Cannot load the selected slot " +
                    "because it does not contain a valid save.");
                return false;
            }

            return LoadPersistentState(slotIndex);
        }

        public bool TryDeleteSaveSlot(int slotIndex)
        {
            if (saveSlotService == null)
            {
                Debug.LogError(
                    "[BootstrapContext] SaveSlotService is not initialized.");
                return false;
            }

            if (!saveSlotService.IsValidSlot(slotIndex))
            {
                Debug.LogWarning(
                    $"[BootstrapContext] Invalid save slot index: {slotIndex}.");
                return false;
            }

            if (!saveSlotService.HasSaveFile(slotIndex))
            {
                Debug.LogWarning(
                    "[BootstrapContext] Cannot delete because the slot " +
                    "does not contain a save file.");
                return false;
            }

            try
            {
                saveSlotService.DeleteSave(slotIndex);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[BootstrapContext] Failed to delete save slot. " +
                    exception);
                return false;
            }
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

        public bool TrySelectSaveSlot(int slotIndex)
        {
            if (saveSlotService == null)
            {
                Debug.LogError(
                    "[BootstrapContext] SaveSlotService is not initialized.");

                return false;
            }

            if (!saveSlotService.IsValidSlot(slotIndex))
            {
                Debug.LogWarning(
                    $"[BootstrapContext] Invalid save slot index: {slotIndex}.");

                return false;
            }

            activeSaveSlotIndex = slotIndex;

            return true;
        }

        public SaveSlotInfo[] GetAllSaveSlotInfo()
        {
            if (saveSlotService == null)
            {
                throw new InvalidOperationException(
                    "SaveSlotService is not initialized.");
            }

            return saveSlotService.GetAllSlotInfo();
        }

#if UNITY_EDITOR

        //==========================================================
        // Validation
        //==========================================================

        private void OnValidate()
        {
            // Persistent Managers
            ValidateReference(runSession, nameof(runSession));
            ValidateReference(runManager, nameof(runManager));
            ValidateReference(poolManager, nameof(poolManager));
            ValidateReference(enemyManager, nameof(enemyManager));
            ValidateReference(levelUpManager, nameof(levelUpManager));
            ValidateReference(gameFlowConfiguration, nameof(gameFlowConfiguration));
            ValidateReference(persistentMetaRuntime, nameof(persistentMetaRuntime));
            ValidateReference(itemTooltipService, nameof(itemTooltipService));
            ValidateReference(gameSettingsRuntime, nameof(gameSettingsRuntime));

            if (string.IsNullOrWhiteSpace(saveFileName))
            {
                Debug.LogWarning(
                    $"{nameof(BootstrapContext)}: " +
                    "'saveFileName' is empty. " +
                    "Defaulting to 'chaosbound_save.json'.",
                    this);
            }

            if (string.IsNullOrWhiteSpace(settingsFileName))
            {
                Debug.LogWarning(
                    $"{nameof(BootstrapContext)}: " +
                    "'settingsFileName' is empty. " +
                    "Defaulting to 'chaosbound_settings.json'.",
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