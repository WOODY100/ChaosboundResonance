using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Gameplay.ExpeditionRuntime.Bootstrap;
using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;

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

    private ExpeditionExitService expeditionExitService;

    public ExpeditionExitService ExpeditionExitService =>
        expeditionExitService;

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

        if (context.GameFlow == null)
        {
            Debug.LogError(
                "GameFlow is not available.");

            return;
        }

        ExpeditionRuntimeBootstrap bootstrap =
            new ExpeditionRuntimeBootstrap(
                sceneTransitionService);

        expeditionDirector =
            bootstrap.Build();

        expeditionExitService =
            bootstrap.BuildExitService(
                expeditionDirector,
                context.GameFlow);
    }

    private void Update()
    {
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null ||
            context.GameFlow == null)
        {
            return;
        }

        if (!context.GameFlow.CanSimulate)
        {
            return;
        }

        expeditionDirector?.Tick();
    }

    public void StartRun(RuntimeExpeditionConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        _currentRunConfig = config;

        expeditionDirector.StartExpedition(
            config);
    }

    public void BindPlayer(PlayerHealth health)
    {
        if (player != null)
            player.OnDeath -= HandlePlayerDeath;

        player = health;

        if (player != null)
            player.OnDeath += HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                "BootstrapContext is not available.",
                this);

            return;
        }

        if (context.GameFlow == null)
        {
            Debug.LogError(
                "GameFlow is not available.",
                this);

            return;
        }

        context.GameFlow.Replace(
            GameFlowContext.GameOver);
    }

    public void AbandonExpedition()
    {
        if (expeditionExitService == null)
        {
            Debug.LogError(
                "ExpeditionExitService is not available.",
                this);

            return;
        }

        expeditionExitService.Exit(
            ExpeditionExitReason.Abandoned);
    }
}