using UnityEngine;
using UnityEngine.UI;
using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;

public sealed class GameFlowConfirmationTestUI :
    MonoBehaviour
{
    [SerializeField]
    private GameObject panelRoot;

    [SerializeField]
    private Button continueButton;

    private GameFlow gameFlow;

    private void Start()
    {
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
        {
            return;
        }

        gameFlow =
            context.GameFlow;

        if (gameFlow == null)
        {

            return;
        }

        gameFlow.OnContextChanged +=
            HandleContextChanged;

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(
                OnContinuePressed);
        }

        UpdateVisibility();
    }

    private void OnDestroy()
    {
        if (gameFlow != null)
        {
            gameFlow.OnContextChanged -=
                HandleContextChanged;
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(
                OnContinuePressed);
        }
    }

    private void HandleContextChanged(
        GameFlowContext previous,
        GameFlowContext current)
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (panelRoot == null ||
            gameFlow == null)
        {
            return;
        }

        panelRoot.SetActive(
            gameFlow.CurrentContext ==
            GameFlowContext.Confirmation);
    }

    private void OnContinuePressed()
    {
        if (gameFlow == null)
            return;

        gameFlow.Pop(
            GameFlowContext.Confirmation);
    }
}