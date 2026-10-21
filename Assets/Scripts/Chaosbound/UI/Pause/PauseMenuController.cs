using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using UnityEngine;
using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public sealed class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button abandonButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private OptionsMenuController optionsController;

    [Header("Save Feedback")]
    [SerializeField]
    private SaveFeedbackController saveFeedbackController;

    [SerializeField, Min(0f)]
    private float minimumSavingDisplayTime = 0.25f;

    [Header("Expedition Exit")]
    [SerializeField]
    private SaveFeedbackController expeditionExitFeedback;

    [SerializeField, Min(0f)]
    private float exitResultDisplayTime = 2f;

    private Coroutine exitCoroutine;
    private bool isExiting;

    [Header("Confirmation")]
    [SerializeField]
    private GenericConfirmationController confirmationController;

    private Coroutine saveFeedbackCoroutine;
    private bool isSaving;

    private GameFlow gameFlow;

    private bool isSubscribed;

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnMainMenuSceneLoaded;

        TryInitializeGameFlow();
    }

    private void Awake()
    {
        if (menuRoot == null)
            menuRoot = gameObject;

        menuRoot.SetActive(false);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);

        if (saveButton != null)
            saveButton.onClick.AddListener(SaveGame);

        if (abandonButton != null)
            abandonButton.onClick.AddListener(ConfirmAbandonExpedition);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ConfirmReturnToMainMenu);
    }

    private void OnEnable()
    {
        // La conexión se realizará desde Update cuando
        // BootstrapContext esté disponible.
    }

    private void OnDisable()
    {
        if (isSubscribed && gameFlow != null)
        {
            gameFlow.OnContextChanged -= OnContextChanged;
            isSubscribed = false;
        }

        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(Resume);

        if (saveButton != null)
            saveButton.onClick.RemoveListener(SaveGame);

        if (abandonButton != null)
            abandonButton.onClick.RemoveListener(ConfirmAbandonExpedition);

        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(ConfirmReturnToMainMenu);

        if (saveFeedbackCoroutine != null)
        {
            StopCoroutine(saveFeedbackCoroutine);
            saveFeedbackCoroutine = null;
        }

        isSaving = false;
    }

    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (gameFlow == null)
        {
            TryInitializeGameFlow();
            return;
        }

        if (!gameFlow.IsInitialized)
            return;

        // Mientras las opciones están abiertas, Escape no debe
        // cerrar la pausa ni reanudar la partida.
        if (optionsController != null && optionsController.IsOpen)
            return;

        // Escape no controla la pausa en el menú principal.
        if (UnityEngine.SceneManagement.SceneManager
            .GetActiveScene().name == "MainMenu")
        {
            return;
        }

        if (gameFlow.CurrentContext == GameFlowContext.Pause)
        {
            Resume();
            return;
        }

        if (gameFlow.CurrentContext != GameFlowContext.Playing)
            return;

        if (gameFlow.CurrentEnvironment != GameFlowEnvironment.Sanctuary &&
            gameFlow.CurrentEnvironment != GameFlowEnvironment.Expedition)
        {
            return;
        }

        gameFlow.Request(GameFlowContext.Pause);
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded
            -= OnMainMenuSceneLoaded;
    }

    private void TryInitializeGameFlow()
    {
        if (gameFlow != null)
            return;

        BootstrapContext context = BootstrapContext.Current;

        if (context == null || context.GameFlow == null)
            return;

        gameFlow = context.GameFlow;
        gameFlow.OnContextChanged += OnContextChanged;
        isSubscribed = true;

        UpdateVisibility();
    }

    public void Resume()
    {
        if (gameFlow == null || !gameFlow.IsInitialized)
            return;

        if (gameFlow.CurrentContext != GameFlowContext.Pause)
            return;

        gameFlow.Pop(GameFlowContext.Pause);
    }

    private void OnContextChanged(
        GameFlowContext previous,
        GameFlowContext current)
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (menuRoot == null || gameFlow == null ||
            !gameFlow.IsInitialized)
            return;

        UpdateEnvironmentButtons();

        menuRoot.SetActive(
            gameFlow.CurrentContext == GameFlowContext.Pause);
    }

    private void UpdateEnvironmentButtons()
    {
        if (gameFlow == null || !gameFlow.IsInitialized)
            return;

        bool isSanctuary =
            gameFlow.CurrentEnvironment == GameFlowEnvironment.Sanctuary;

        if (saveButton != null)
            saveButton.gameObject.SetActive(isSanctuary);

        if (optionsButton != null)
            optionsButton.gameObject.SetActive(isSanctuary);

        if (abandonButton != null)
            abandonButton.gameObject.SetActive(!isSanctuary);

        if (mainMenuButton != null)
            mainMenuButton.gameObject.SetActive(true);
    }

    private void SaveGame()
    {
        if (isSaving || gameFlow == null || !gameFlow.IsInitialized)
            return;

        if (gameFlow.CurrentContext != GameFlowContext.Pause ||
            gameFlow.CurrentEnvironment != GameFlowEnvironment.Sanctuary)
            return;

        BootstrapContext context = BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                "[PauseMenuController] BootstrapContext is missing.",
                this);

            if (saveFeedbackController != null)
                saveFeedbackController.ShowResult(false);

            return;
        }

        if (saveFeedbackController == null)
        {
            Debug.LogError(
                "[PauseMenuController] SaveFeedbackController is missing.",
                this);

            return;
        }

        saveFeedbackCoroutine = StartCoroutine(SaveGameRoutine(context));
    }

    private IEnumerator SaveGameRoutine(BootstrapContext context)
    {
        isSaving = true;

        saveFeedbackController.ShowSaving();

        // Permite renderizar el indicador antes de guardar.
        yield return null;

        float elapsed = 0f;

        while (elapsed < minimumSavingDisplayTime)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        bool success = context.SavePersistentState();

        saveFeedbackController.ShowResult(success);

        saveFeedbackCoroutine = null;
        isSaving = false;
    }

    public void OpenOptions()
    {
        if (gameFlow == null || !gameFlow.IsInitialized)
            return;

        if (gameFlow.CurrentContext != GameFlowContext.Pause)
            return;

        if (gameFlow.CurrentEnvironment != GameFlowEnvironment.Sanctuary)
            return;

        if (optionsController == null)
        {
            Debug.LogError(
                "[PauseMenuController] OptionsMenuController is missing.",
                this);
            return;
        }

        optionsController.OpenOptionsFromPause();
    }

    private void ConfirmAbandonExpedition()
    {
        if (!CanRequestExpeditionExit())
            return;

        if (confirmationController == null)
        {
            Debug.LogError(
                "[PauseMenuController] ConfirmationController is missing.",
                this);
            return;
        }

        confirmationController.Show(
            "¿Abandonar expedición?",
            "¿Seguro que deseas abandonar la expedición actual? " +
            "Los objetos protegidos se conservarán según las reglas actuales.",
            "CANCELAR",
            "ABANDONAR",
            AbandonExpedition);
    }

    private void ConfirmReturnToMainMenu()
    {
        if (gameFlow == null || !gameFlow.IsInitialized)
            return;

        if (gameFlow.CurrentContext != GameFlowContext.Pause)
            return;

        if (confirmationController == null)
        {
            Debug.LogError(
                "[PauseMenuController] ConfirmationController is missing.",
                this);
            return;
        }

        confirmationController.Show(
            "¿Volver al menú principal?",
            "Si continúas, saldrás de la expedición actual si hay una activa. " +
            "El progreso no completado no se registrará como una expedición completada.",
            "CANCELAR",
            "SALIR",
            ReturnToMainMenu);
    }

    private bool CanRequestExpeditionExit()
    {
        if (gameFlow == null || !gameFlow.IsInitialized)
            return false;

        if (gameFlow.CurrentContext != GameFlowContext.Pause)
            return false;

        if (gameFlow.CurrentEnvironment != GameFlowEnvironment.Expedition)
            return false;

        RunManager runManager = RunManager.Instance;

        if (runManager == null ||
            runManager.ExpeditionExitService == null)
        {
            Debug.LogError(
                "[PauseMenuController] Expedition exit service is unavailable.",
                this);
            return false;
        }

        return true;
    }

    private void AbandonExpedition()
    {
        if (isExiting || !CanRequestExpeditionExit())
            return;

        RunManager runManager = RunManager.Instance;

        if (runManager == null ||
            runManager.ExpeditionExitService == null)
            return;

        ClosePauseMenu();

        exitCoroutine = StartCoroutine(
            ExitExpeditionRoutine(
                runManager,
                ExpeditionExitDestination.Sanctuary));
    }

    private void ClosePauseMenu()
    {
        if (optionsController != null && optionsController.IsOpen)
            optionsController.CloseOptions();

        if (confirmationController != null)
            confirmationController.Hide();

        if (menuRoot != null)
            menuRoot.SetActive(false);
    }

    private void ReturnToMainMenu()
    {
        if (gameFlow == null || !gameFlow.IsInitialized)
            return;

        if (gameFlow.CurrentContext != GameFlowContext.Pause)
            return;

        BootstrapContext context = BootstrapContext.Current;

        if (context == null || context.SceneTransitionService == null)
        {
            Debug.LogError(
                "[PauseMenuController] SceneTransitionService is unavailable.",
                this);
            return;
        }

        GameFlowEnvironment environment = gameFlow.CurrentEnvironment;

        if (environment == GameFlowEnvironment.Sanctuary)
        {
            ClosePauseMenu();

            gameFlow.ResetFlow();
            gameFlow.SetEnvironment(GameFlowEnvironment.Sanctuary);

            context.SceneTransitionService.LoadScene(
                Chaosbound.Core.Runtime.SceneManagement.GameScene.MainMenu);

            gameFlow.Initialize();
            return;
        }

        if (environment == GameFlowEnvironment.Expedition)
        {
            RunManager runManager = RunManager.Instance;

            if (runManager == null ||
                runManager.ExpeditionExitService == null)
            {
                Debug.LogError(
                    "[PauseMenuController] ExpeditionExitService is unavailable.",
                    this);
                return;
            }

            if (isExiting)
                return;

            ClosePauseMenu();

            exitCoroutine = StartCoroutine(
                ExitExpeditionRoutine(
                    runManager,
                    ExpeditionExitDestination.MainMenu));
        }
    }

    private void OnMainMenuSceneLoaded(
        UnityEngine.SceneManagement.Scene scene,
        UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
            return;

        ClosePauseMenu();
    }

    private IEnumerator ExitExpeditionRoutine(
        RunManager runManager,
        ExpeditionExitDestination destination)
    {
        isExiting = true;

        if (expeditionExitFeedback == null)
        {
            Debug.LogError(
                "[PauseMenuController] Expedition exit feedback is missing.",
                this);

            isExiting = false;
            yield break;
        }

        expeditionExitFeedback.ShowSaving();

        // Permite que Unity renderice el indicador.
        yield return null;

        bool success = runManager.ExpeditionExitService.PrepareExit(
            ExpeditionExitReason.Abandoned,
            runManager.CurrentRunConfig);

        expeditionExitFeedback.ShowResult(success);

        if (!success)
        {
            isExiting = false;
            yield break;
        }

        // Da tiempo a que se vea la confirmación de guardado.
        yield return new WaitForSecondsRealtime(exitResultDisplayTime);

        runManager.ExpeditionExitService.CompleteExit(destination);

        isExiting = false;
        exitCoroutine = null;
    }
}
