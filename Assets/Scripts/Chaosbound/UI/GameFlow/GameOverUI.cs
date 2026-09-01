using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameOverUI : MonoBehaviour
{
    //==========================================================
    // References
    //==========================================================

    [Header("References")]

    [SerializeField]
    private GameObject panelRoot;

    [SerializeField]
    private Button exitButton;

    //==========================================================
    // Runtime
    //==========================================================

    private GameFlow gameFlow;

    //==========================================================
    // Unity
    //==========================================================

    private void Awake()
    {
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(
                HandleExitClicked);
        }
    }

    private void Start()
    {
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                "GameOverUI: BootstrapContext is not available.",
                this);

            return;
        }

        gameFlow =
            context.GameFlow;

        if (gameFlow == null)
        {
            Debug.LogError(
                "GameOverUI: GameFlow is not available.",
                this);

            return;
        }

        gameFlow.OnContextChanged +=
            HandleContextChanged;

        SynchronizeWithCurrentContext();
    }

    private void OnDestroy()
    {
        if (gameFlow != null)
        {
            gameFlow.OnContextChanged -=
                HandleContextChanged;
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(
                HandleExitClicked);
        }
    }

    //==========================================================
    // Context
    //==========================================================

    private void HandleContextChanged(
        GameFlowContext previous,
        GameFlowContext current)
    {
        SetVisible(
            current == GameFlowContext.GameOver);
    }

    private void SynchronizeWithCurrentContext()
    {
        if (gameFlow == null)
            return;

        if (!gameFlow.IsInitialized)
        {
            SetVisible(false);
            return;
        }

        SetVisible(
            gameFlow.CurrentContext ==
            GameFlowContext.GameOver);
    }

    private void SetVisible(bool visible)
    {
        if (panelRoot != null)
            panelRoot.SetActive(visible);
    }

    //==========================================================
    // Exit
    //==========================================================

    private void HandleExitClicked()
    {
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                "GameOverUI: BootstrapContext is not available.",
                this);

            return;
        }

        RunManager runManager =
            context.RunManager;

        if (runManager == null)
        {
            Debug.LogError(
                "GameOverUI: RunManager is not available.",
                this);

            return;
        }

        if (runManager.ExpeditionExitService == null)
        {
            Debug.LogError(
                "GameOverUI: ExpeditionExitService is not available.",
                this);

            return;
        }

        runManager.ExpeditionExitService.Exit(
            ExpeditionExitReason.Death);
    }
}