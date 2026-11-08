using Chaosbound.Gameplay.Inventory.UI;
using Chaosbound.Gameplay.Items.Testing;
using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.Settlement;
using Chaosbound.Gameplay.MetaProgression.Persistent;
using Chaosbound.Gameplay.Currency.Persistent;
using Chaosbound.Core.Composition;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatsDebugTest : MonoBehaviour
{
    private PlayerExperienceSystem experience;

    [Header("Blacksmith Debug")]

    [SerializeField]
    private GameObject blacksmithWindow;

    [SerializeField]
    private BlacksmithInventoryUI blacksmithInventoryUI;

    [Header("Meta Progression Debug")]

    [SerializeField]
    private int debugMetaExperience = 500;

    [Header("Expedition Selection Debug")]

    [Header("Currency Debug")]

    [SerializeField]
    private int debugGoldAmount = 1000;

    private const string GoldContentId = "gold";
    private PersistentCurrencyState persistentCurrencyState;

    private PersistentMetaRuntime persistentMetaRuntime;

    private WorldItemTestSpawner worldItemTestSpawner;

    private void Awake()
    {
        BootstrapContext context = BootstrapContext.Current;

        if (context != null)
        {
            persistentCurrencyState = context.PersistentCurrencyState;
        }

        experience =
            GetComponent<PlayerExperienceSystem>();

        persistentMetaRuntime =
            FindFirstObjectByType<PersistentMetaRuntime>();

        if (persistentMetaRuntime == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "PersistentMetaRuntime could not be found.",
                this);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // L = Add XP
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            experience.AddXP(50f);
        }

        // P = Add Meta XP
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            AddDebugMetaExperience();
        }

        // K = Spawn test item
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            worldItemTestSpawner?.SpawnAnotherItem();
        }

        // J = Abandon Expedition
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            AbandonExpedition();
        }

        // H = Double Settlement Test
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            TestDoubleSettlement();
        }

        // U = Open Blacksmith Window
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            OpenBlacksmithWindow();
        }

        // F = Add Debug Gold
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            AddDebugGold();
        }
    }

    private void AddDebugGold()
    {
        if (persistentCurrencyState == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "PersistentCurrencyState is not available.",
                this);

            return;
        }

        if (debugGoldAmount <= 0)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "Debug Gold Amount must be greater than zero.",
                this);

            return;
        }

        int goldBefore =
            persistentCurrencyState.GetAmount(GoldContentId);

        if (!persistentCurrencyState.Add(
                GoldContentId,
                debugGoldAmount))
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "Could not add debug Gold.",
                this);

            return;
        }

        int goldAfter =
            persistentCurrencyState.GetAmount(GoldContentId);

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"Debug Gold Added: {debugGoldAmount} | " +
            $"Before: {goldBefore} | After: {goldAfter}",
            this);
    }

    private void AddDebugMetaExperience()
    {
        if (persistentMetaRuntime == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "PersistentMetaRuntime is not assigned.",
                this);

            return;
        }

        if (debugMetaExperience <= 0)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "Debug Meta Experience must be greater than zero.",
                this);

            return;
        }

        PersistentMetaState state =
            persistentMetaRuntime.State;

        if (state == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "PersistentMetaState is not available.",
                this);

            return;
        }

        int experienceBefore =
            state.Experience;

        int levelBefore =
            state.MetaLevel;

        int skillPointsBefore =
            state.SkillPoints;

        persistentMetaRuntime.AddMetaExperience(
            debugMetaExperience);

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "========================================");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "META XP DEBUG TEST");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"XP Before = {experienceBefore}");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"Meta Level Before = {levelBefore}");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"Skill Points Before = {skillPointsBefore}");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"Meta XP Added = {debugMetaExperience}");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"XP After = {state.Experience}");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"Meta Level After = {state.MetaLevel}");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"Skill Points After = {state.SkillPoints}");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "========================================");
    }

    private void AbandonExpedition()
    {
        if (RunManager.Instance == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "RunManager.Instance is null.",
                this);

            return;
        }

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "Debug abandon requested.");

        RunManager.Instance.AbandonExpedition();
    }

    private void TestDoubleSettlement()
    {
        if (RunManager.Instance == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "RunManager.Instance is null.",
                this);

            return;
        }

        ExpeditionSettlementService settlementService =
            RunManager.Instance.ExpeditionSettlementService;

        if (settlementService == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "ExpeditionSettlementService is not available.",
                this);

            return;
        }

        if (RunManager.Instance.ExpeditionRuntimeState == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "ExpeditionRuntimeState is not available.",
                this);

            return;
        }

        if (RunManager.Instance.CurrentRunConfig == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "CurrentRunConfig is not available.",
                this);

            return;
        }

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "========================================");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "DOUBLE SETTLEMENT TEST — FIRST ATTEMPT");

        bool firstSuccess =
            settlementService.TrySettle(
                RunManager.Instance.ExpeditionRuntimeState,
                RunManager.Instance.CurrentRunConfig,
                out ExpeditionSettlementResult firstResult);

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"First Settlement Success = {firstSuccess}");

        if (firstResult != null)
        {
            Debug.Log(
                "[PlayerStatsDebugTest] " +
                $"First Result Success = {firstResult.Success}");
        }

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "DOUBLE SETTLEMENT TEST — SECOND ATTEMPT");

        bool secondSuccess =
            settlementService.TrySettle(
                RunManager.Instance.ExpeditionRuntimeState,
                RunManager.Instance.CurrentRunConfig,
                out ExpeditionSettlementResult secondResult);

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"Second Settlement Success = {secondSuccess}");

        if (secondResult != null)
        {
            Debug.Log(
                "[PlayerStatsDebugTest] " +
                $"Second Result Success = {secondResult.Success}");
        }

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            $"Settlement Service IsSettled = " +
            $"{settlementService.IsSettled}");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "========================================");

        if (!firstSuccess)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "Double Settlement Test failed: " +
                "first settlement did not succeed.",
                this);

            return;
        }

        if (secondSuccess)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "DOUBLE SETTLEMENT BUG: " +
                "second settlement succeeded.",
                this);

            return;
        }

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "DOUBLE SETTLEMENT TEST PASSED.");

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "First settlement committed successfully. " +
            "Second settlement was rejected.");

        // The first settlement has already been committed.
        // We now leave the Expedition through the Abandoned
        // exit path only to perform normal runtime cleanup.
        RunManager.Instance.ExpeditionExitService.Exit(
            ExpeditionExitReason.Abandoned,
            RunManager.Instance.CurrentRunConfig);
    }

    private void OpenBlacksmithWindow()
    {
        if (blacksmithWindow == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "BlacksmithWindow is not assigned.",
                this);

            return;
        }

        if (blacksmithInventoryUI == null)
        {
            Debug.LogError(
                "[PlayerStatsDebugTest] " +
                "BlacksmithInventoryUI is not assigned.",
                this);

            return;
        }

        blacksmithWindow.SetActive(true);

        blacksmithInventoryUI.RefreshInventory();

        Debug.Log(
            "[PlayerStatsDebugTest] " +
            "Blacksmith Window opened.");
    }
}