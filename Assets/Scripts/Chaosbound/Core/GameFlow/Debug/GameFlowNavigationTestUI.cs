using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameFlowNavigationTestUI : MonoBehaviour
{
    [Header("Panels")]

    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private GameObject statsPanel;

    [SerializeField]
    private GameObject confirmationPanel;

    [Header("Pause Buttons")]

    [SerializeField]
    private Button pauseInventoryButton;

    [SerializeField]
    private Button pauseStatsButton;

    [SerializeField]
    private Button pauseResumeButton;

    [Header("Inventory Buttons")]

    [SerializeField]
    private Button inventoryStatsButton;

    [SerializeField]
    private Button inventoryBackButton;

    [Header("Stats Buttons")]

    [SerializeField]
    private Button statsInventoryButton;

    [SerializeField]
    private Button statsBackButton;

    private GameFlow gameFlow;

    [SerializeField]
    private Button pauseExitButton;

    [Header("Confirmation Buttons")]

    [SerializeField]
    private Button confirmationContinueButton;

    [SerializeField]
    private Button confirmationCancelButton;

    //==========================================================
    // Unity
    //==========================================================

    private void Start()
    {
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                $"{nameof(GameFlowNavigationTestUI)} " +
                $"could not find {nameof(BootstrapContext)}.",
                this);

            return;
        }

        gameFlow =
            context.GameFlow;

        if (gameFlow == null)
        {
            Debug.LogError(
                $"{nameof(GameFlowNavigationTestUI)} " +
                $"could not find {nameof(GameFlow)}.",
                this);

            return;
        }

        gameFlow.OnContextChanged +=
            HandleContextChanged;

        RegisterButtonListeners();

        UpdatePanels(
            gameFlow.CurrentContext);
    }

    private void OnDestroy()
    {
        if (gameFlow != null)
        {
            gameFlow.OnContextChanged -=
                HandleContextChanged;
        }

        UnregisterButtonListeners();
    }

    //==========================================================
    // Button Registration
    //==========================================================

    private void RegisterButtonListeners()
    {
        pauseInventoryButton?.onClick.AddListener(
            OpenInventory);

        pauseStatsButton?.onClick.AddListener(
            OpenStats);

        pauseResumeButton?.onClick.AddListener(
            Resume);

        inventoryStatsButton?.onClick.AddListener(
            OpenStats);

        inventoryBackButton?.onClick.AddListener(
            BackFromInventory);

        statsInventoryButton?.onClick.AddListener(
            OpenInventory);

        statsBackButton?.onClick.AddListener(
            BackFromStats);

        pauseExitButton?.onClick.AddListener(
            OpenConfirmation);

        confirmationContinueButton?.onClick.AddListener(
            ConfirmAbandon);

        confirmationCancelButton?.onClick.AddListener(
            CancelConfirmation);
    }

    private void UnregisterButtonListeners()
    {
        pauseInventoryButton?.onClick.RemoveListener(
            OpenInventory);

        pauseStatsButton?.onClick.RemoveListener(
            OpenStats);

        pauseResumeButton?.onClick.RemoveListener(
            Resume);

        inventoryStatsButton?.onClick.RemoveListener(
            OpenStats);

        inventoryBackButton?.onClick.RemoveListener(
            BackFromInventory);

        statsInventoryButton?.onClick.RemoveListener(
            OpenInventory);

        statsBackButton?.onClick.RemoveListener(
            BackFromStats);

        pauseExitButton?.onClick.RemoveListener(
            OpenConfirmation);

        confirmationContinueButton?.onClick.RemoveListener(
            ConfirmAbandon);

        confirmationCancelButton?.onClick.RemoveListener(
            CancelConfirmation);
    }

    //==========================================================
    // Navigation
    //==========================================================

    private void OpenInventory()
    {
        if (gameFlow == null)
            return;

        gameFlow.Request(
            GameFlowContext.Inventory);
    }

    private void OpenStats()
    {
        if (gameFlow == null)
            return;

        gameFlow.Request(
            GameFlowContext.Stats);
    }

    private void Resume()
    {
        if (gameFlow == null)
            return;

        gameFlow.Pop(
            GameFlowContext.Pause);
    }

    private void BackFromInventory()
    {
        if (gameFlow == null)
            return;

        gameFlow.Pop(
            GameFlowContext.Inventory);
    }

    private void BackFromStats()
    {
        if (gameFlow == null)
            return;

        gameFlow.Pop(
            GameFlowContext.Stats);
    }

    //==========================================================
    // GameFlow
    //==========================================================

    private void HandleContextChanged(
        GameFlowContext previous,
        GameFlowContext current)
    {
        UpdatePanels(current);
    }

    private void UpdatePanels(
        GameFlowContext context)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(
                context == GameFlowContext.Pause);
        }

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(
                context == GameFlowContext.Inventory);
        }

        if (statsPanel != null)
        {
            statsPanel.SetActive(
                context == GameFlowContext.Stats);
        }

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(
                context == GameFlowContext.Confirmation);
        }
    }

    private void OpenConfirmation()
    {
        if (gameFlow == null)
            return;

        gameFlow.Request(
            GameFlowContext.Confirmation);
    }

    private void ConfirmAbandon()
    {
        RunManager runManager =
            RunManager.Instance;

        if (runManager == null)
        {
            Debug.LogError(
                "RunManager is not available.");

            return;
        }

        ExpeditionExitService exitService =
            runManager.ExpeditionExitService;

        if (exitService == null)
        {
            Debug.LogError(
                "ExpeditionExitService is not available.");

            return;
        }

        exitService.Exit(
            ExpeditionExitReason.Abandoned);
    }

    private void CancelConfirmation()
    {
        if (gameFlow == null)
            return;

        gameFlow.Pop(
            GameFlowContext.Confirmation);
    }
}