using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
        Debug.Log("Continue game pending.");
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