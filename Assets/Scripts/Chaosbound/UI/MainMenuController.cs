using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Chaosbound.Core.Composition;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private string gameplaySceneName = "Gameplay";

    private IEnumerator Start()
    {
        yield return null;

        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
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

        SceneManager.LoadScene(gameplaySceneName);
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