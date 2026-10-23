using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Chaosbound.Core.Composition;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;

    [SerializeField]
    private SaveSlotPanelController saveSlotPanelController;

    private IEnumerator Start()
    {
        yield return null;

        if (firstSelectedButton != null &&
            EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(
                firstSelectedButton);
        }
    }


    public void StartGame()
    {
        OpenSaveSlotPanel(true);
    }

    public void ContinueGame()
    {
        OpenSaveSlotPanel(false);
    }

    private void OpenSaveSlotPanel(bool startNewGame)
    {
        if (saveSlotPanelController == null)
        {
            Debug.LogError(
                "[MainMenuController] SaveSlotPanelController " +
                "is not assigned.",
                this);
            return;
        }

        saveSlotPanelController.OpenPanel(startNewGame);
    }


    public void OpenOptions()
    {
        BootstrapContext bootstrapContext =
            BootstrapContext.Current;

        if (bootstrapContext == null)
        {
            Debug.LogError(
                "[MainMenuController] BootstrapContext is not available.",
                this);
            return;
        }

        OptionsMenuController optionsController =
            bootstrapContext.GetComponentInChildren
                <OptionsMenuController>(true);

        if (optionsController == null)
        {
            Debug.LogError(
                "[MainMenuController] OptionsMenuController " +
                "was not found in Bootstrap.",
                this);
            return;
        }

        optionsController.OpenOptionsFromMainMenu();
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
