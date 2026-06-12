using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private GameObject optionsPanel;

    [Header("Tabs")]
    [SerializeField] private GameObject videoContent;
    [SerializeField] private GameObject audioContent;
    [SerializeField] private GameObject gameplayContent;
    [SerializeField] private GameObject controlsContent;

    [Header("Selection")]
    [SerializeField] private GameObject firstOptionsButton;
    [SerializeField] private GameObject optionsMenuButton;

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);

        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.alpha = 0.35f;
            mainMenuCanvasGroup.interactable = false;
            mainMenuCanvasGroup.blocksRaycasts = false;
        }

        ShowVideoTab();

        if (firstOptionsButton != null)
            EventSystem.current.SetSelectedGameObject(firstOptionsButton);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);

        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.alpha = 1f;
            mainMenuCanvasGroup.interactable = true;
            mainMenuCanvasGroup.blocksRaycasts = true;
        }

        if (optionsMenuButton != null)
            EventSystem.current.SetSelectedGameObject(optionsMenuButton);
    }

    public void ShowVideoTab()
    {
        SetTab(videoContent);
    }

    public void ShowAudioTab()
    {
        SetTab(audioContent);
    }

    public void ShowGameplayTab()
    {
        SetTab(gameplayContent);
    }

    public void ShowControlsTab()
    {
        SetTab(controlsContent);
    }

    private void SetTab(GameObject activeTab)
    {
        videoContent.SetActive(activeTab == videoContent);
        audioContent.SetActive(activeTab == audioContent);
        gameplayContent.SetActive(activeTab == gameplayContent);
        controlsContent.SetActive(activeTab == controlsContent);
    }
}