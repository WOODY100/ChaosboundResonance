using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Chaosbound.Core.Audio;
using Chaosbound.Core.Settings.UI;

public sealed class OptionsMenuController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Panel Positions")]
    [SerializeField]
    private Vector2 mainMenuPosition =
        new Vector2(-15f, -15f);

    [SerializeField]
    private Vector2 pauseMenuPosition =
        Vector2.zero;

    [Header("Tabs")]
    [SerializeField] private GameObject videoContent;
    [SerializeField] private GameObject audioContent;
    [SerializeField] private GameObject gameplayContent;
    [SerializeField] private GameObject controlsContent;

    [Header("Tab Controllers")]
    [SerializeField] private VideoSettingsUI videoSettingsUI;
    [SerializeField] private GameplaySettingsUI gameplaySettingsUI;
    [SerializeField] private AudioSettingsUI audioSettingsUI;

    [Header("Footer Buttons")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button restoreButton;

    [Header("Selection")]
    [SerializeField] private GameObject firstOptionsButton;

    [Header("UI Audio")]
    [SerializeField] private UIAudioPlayer uiAudioPlayer;

    private bool isOpen;
    private GameObject activeTab;
    private RectTransform optionsRectTransform;

    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalPivot;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (optionsPanel == null)
        {
            Debug.LogError(
                "[OptionsMenuController] Options Panel is not assigned.",
                this);
            return;
        }

        optionsRectTransform =
            optionsPanel.GetComponent<RectTransform>();

        if (optionsRectTransform == null)
        {
            Debug.LogError(
                "[OptionsMenuController] Options Panel has no RectTransform.",
                this);
            return;
        }

        // Capturamos la configuración original del prefab.
        originalAnchorMin = optionsRectTransform.anchorMin;
        originalAnchorMax = optionsRectTransform.anchorMax;
        originalPivot = optionsRectTransform.pivot;

        ApplyMainMenuPosition();
    }

    public void OpenOptions()
    {
        OpenOptionsFromMainMenu();
    }

    public void OpenOptionsFromMainMenu()
    {
        OpenOptionsInternal(false);
    }

    public void OpenOptionsFromPause()
    {
        OpenOptionsInternal(true);
    }

    private void OpenOptionsInternal(bool centerPanel)
    {
        if (optionsPanel == null || isOpen)
            return;

        if (centerPanel)
            ApplyPauseMenuPosition();
        else
            ApplyMainMenuPosition();

        isOpen = true;
        optionsPanel.SetActive(true);

        ShowVideoTab();

        if (firstOptionsButton != null &&
            EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(
                firstOptionsButton);
        }
    }

    public void CloseOptions()
    {
        if (optionsPanel == null || !isOpen)
            return;

        isOpen = false;
        optionsPanel.SetActive(false);

        if (uiAudioPlayer != null)
            uiAudioPlayer.PlayPopup();

        // Restauramos la posición y geometría originales.
        ApplyMainMenuPosition();

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void ApplyMainMenuPosition()
    {
        if (optionsRectTransform == null)
            return;

        optionsRectTransform.anchorMin = originalAnchorMin;
        optionsRectTransform.anchorMax = originalAnchorMax;
        optionsRectTransform.pivot = originalPivot;

        optionsRectTransform.anchoredPosition =
            mainMenuPosition;
    }

    private void ApplyPauseMenuPosition()
    {
        if (optionsRectTransform == null)
            return;

        optionsRectTransform.anchorMin =
            new Vector2(0.5f, 0.5f);

        optionsRectTransform.anchorMax =
            new Vector2(0.5f, 0.5f);

        optionsRectTransform.pivot =
            new Vector2(0.5f, 0.5f);

        optionsRectTransform.anchoredPosition =
            pauseMenuPosition;
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

    public void ApplyActiveTab()
    {
        if (activeTab == videoContent)
        {
            if (videoSettingsUI != null)
                videoSettingsUI.Apply();
            else
                Debug.LogError(
                    "[OptionsMenuController] VideoSettingsUI " +
                    "is not assigned.",
                    this);

            return;
        }

        if (activeTab == audioContent)
        {
            if (audioSettingsUI != null)
                audioSettingsUI.Apply();
            else
                Debug.LogError(
                    "[OptionsMenuController] AudioSettingsUI " +
                    "is not assigned.",
                    this);

            return;
        }

        if (activeTab == gameplayContent)
        {
            if (gameplaySettingsUI != null)
                gameplaySettingsUI.Apply();
            else
                Debug.LogError(
                    "[OptionsMenuController] GameplaySettingsUI " +
                    "is not assigned.",
                    this);

            return;
        }

        Debug.Log(
            "[OptionsMenuController] Apply is not configured " +
            "for the active tab.",
            this);
    }

    public void RestoreActiveTab()
    {
        if (activeTab == videoContent)
        {
            if (videoSettingsUI != null)
                videoSettingsUI.RestoreDefaults();
            else
                Debug.LogError(
                    "[OptionsMenuController] VideoSettingsUI " +
                    "is not assigned.",
                    this);

            return;
        }

        if (activeTab == audioContent)
        {
            if (audioSettingsUI != null)
                audioSettingsUI.RestoreDefaults();
            else
                Debug.LogError(
                    "[OptionsMenuController] AudioSettingsUI " +
                    "is not assigned.",
                    this);

            return;
        }

        if (activeTab == gameplayContent)
        {
            if (gameplaySettingsUI != null)
                gameplaySettingsUI.RestoreDefaults();
            else
                Debug.LogError(
                    "[OptionsMenuController] GameplaySettingsUI " +
                    "is not assigned.",
                    this);

            return;
        }

        Debug.Log(
            "[OptionsMenuController] Restore is not configured " +
            "for the active tab.",
            this);
    }


    private void SetTab(GameObject tab)
    {
        if (tab == null)
            return;

        // No reproducir sonido si la pestaña ya está activa.
        if (activeTab == tab)
            return;

        activeTab = tab;

        if (videoContent != null)
            videoContent.SetActive(tab == videoContent);

        if (audioContent != null)
            audioContent.SetActive(tab == audioContent);

        if (gameplayContent != null)
            gameplayContent.SetActive(tab == gameplayContent);

        if (controlsContent != null)
            controlsContent.SetActive(tab == controlsContent);

        if (uiAudioPlayer != null)
            uiAudioPlayer.PlayTabSwitch();
    }

}