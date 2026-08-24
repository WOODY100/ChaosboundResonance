using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Gameplay.ExpeditionRuntime.Bootstrap;
using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Core.Composition;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.UI.Timeline;
using System;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;

    [SerializeField] private GameObject gameOverPanel;

    [SerializeField]
    private TimelineUI timelineUI;

    private PlayerHealth player;

    private ExpeditionDirector expeditionDirector;

    public ExpeditionDirector ExpeditionDirector =>
        expeditionDirector;

    public ExpeditionRuntimeState ExpeditionRuntimeState =>
    expeditionDirector != null
        ? expeditionDirector.RuntimeState
        : null;

    private RuntimeExpeditionConfig _currentRunConfig;

    public RuntimeExpeditionConfig CurrentRunConfig => _currentRunConfig;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                "BootstrapContext is not available.");

            return;
        }

        SceneTransitionService sceneTransitionService =
            context.SceneTransitionService;

        if (sceneTransitionService == null)
        {
            Debug.LogError(
                "SceneTransitionService is not available.");

            return;
        }

        ExpeditionRuntimeBootstrap bootstrap =
            new ExpeditionRuntimeBootstrap(
                sceneTransitionService);

        expeditionDirector =
            bootstrap.Build();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (GameStateManager.Instance == null)
        {
            return;
        }

        if (!GameStateManager.Instance.CanSimulate)
        {
            return;
        }

        expeditionDirector?.Tick();

        if (timelineUI != null &&
            expeditionDirector != null &&
            expeditionDirector.IsRunning)
        {
            timelineUI.UpdateProgress(
                (float)
                expeditionDirector.RuntimeState
                    .ElapsedTime
                    .TotalSeconds);
        }
    }

    public void StartRun(RuntimeExpeditionConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        _currentRunConfig = config;

        Time.timeScale = 1f;

        expeditionDirector.StartExpedition(
            config);

        if (timelineUI != null &&
            config.Timeline != null &&
            config.Timeline.Agenda != null)
        {
            timelineUI.SetAgenda(
                config.Timeline.Agenda);
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void BindPlayer(PlayerHealth health)
    {
        if (player != null)
            player.OnDeath -= HandlePlayerDeath;

        player = health;

        if (player != null)
            player.OnDeath += HandlePlayerDeath;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void HandlePlayerDeath()
    {
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                "BootstrapContext is not available.");

            return;
        }

        expeditionDirector?.AbortExpedition();

        SceneTransitionService sceneTransitionService =
            context.SceneTransitionService;

        if (sceneTransitionService == null)
        {
            Debug.LogError(
                "SceneTransitionService is not available.");

            return;
        }

        sceneTransitionService.LoadScene(
            GameScene.Expedition);
    }
}