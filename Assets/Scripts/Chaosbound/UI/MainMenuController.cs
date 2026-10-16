
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Chaosbound.Core.Composition;
using Chaosbound.Core.Runtime.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;

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
        BootstrapContext bootstrapContext =
            BootstrapContext.Current;

        if (bootstrapContext == null)
        {
            Debug.LogError(
                "[MainMenuController] " +
                "BootstrapContext is not available.");

            return;
        }

        if (bootstrapContext.SceneTransitionService == null)
        {
            Debug.LogError(
                "[MainMenuController] " +
                "SceneTransitionService is not available.");

            return;
        }

        bootstrapContext.SceneTransitionService.LoadScene(
            GameScene.Sanctuary);
    }

    public void ContinueGame()
    {
        BootstrapContext bootstrapContext =
            BootstrapContext.Current;

        if (bootstrapContext == null)
        {
            Debug.LogError(
                "[MainMenuController] " +
                "BootstrapContext is not available.");

            return;
        }

        if (bootstrapContext.SceneTransitionService == null)
        {
            Debug.LogError(
                "[MainMenuController] " +
                "SceneTransitionService is not available.");

            return;
        }

        bool loaded =
            bootstrapContext.LoadPersistentState();

        if (!loaded)
        {
            Debug.LogWarning(
                "[MainMenuController] " +
                "No valid save game was found. " +
                "Continue cancelled.");

            return;
        }

        bootstrapContext.SceneTransitionService.LoadScene(
            GameScene.Sanctuary);
    }

    public void OpenOptions()
    {
        Debug.Log("Options menu pending.");
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
