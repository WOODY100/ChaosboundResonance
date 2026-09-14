using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.Settlement;
using Chaosbound.Gameplay.Items.Testing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatsDebugTest : MonoBehaviour
{
    private PlayerExperienceSystem experience;

    [SerializeField]
    private WorldItemTestSpawner worldItemTestSpawner;

    private void Awake()
    {
        experience =
            GetComponent<PlayerExperienceSystem>();
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
}