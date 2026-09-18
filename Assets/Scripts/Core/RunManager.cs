using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Gameplay.ExpeditionRuntime.Bootstrap;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Settlement;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.World.Integration;
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

    private ItemWorldDropService itemWorldDropService;

    public ItemWorldDropService ItemWorldDropService =>
        itemWorldDropService;

    private ItemWorldDropConfirmationService
        itemWorldDropConfirmationService;

    public ItemWorldDropConfirmationService
        ItemWorldDropConfirmationService =>
            itemWorldDropConfirmationService;

    private ExpeditionSettlementService
        expeditionSettlementService;

    public ExpeditionSettlementService
        ExpeditionSettlementService =>
            expeditionSettlementService;

    private ExpeditionSecurePreservationService
        expeditionSecurePreservationService;

    public ExpeditionSecurePreservationService
        ExpeditionSecurePreservationService =>
            expeditionSecurePreservationService;

    private void Awake()
    {
        Instance = this;
    }

    public void InitializeExpeditionRuntime()
    {
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
        {
            throw new InvalidOperationException(
                "BootstrapContext is not available.");
        }

        SceneTransitionService sceneTransitionService =
            context.SceneTransitionService;

        if (sceneTransitionService == null)
        {
            throw new InvalidOperationException(
                "SceneTransitionService is not available.");
        }

        GameContentContext compositionContext =
            GameContentContext.Current;

        if (context.PersistentInventoryRuntime == null)
        {
            throw new InvalidOperationException(
                "PersistentInventoryRuntime is not available.");
        }

        if (context.PersistentMetaRuntime == null)
        {
            throw new InvalidOperationException(
                "PersistentMetaRuntime is not available.");
        }

        if (compositionContext.ExpeditionRewardItemDatabase == null)
        {
            throw new InvalidOperationException(
                "ExpeditionRewardItemDatabase is not available.");
        }

        if (compositionContext == null)
        {
            throw new InvalidOperationException(
                "GameContentContext is not available.");
        }

        if (context.GameFlow == null)
        {
            throw new InvalidOperationException(
                "GameFlow is not available.");
        }

        ExpeditionRuntimeBootstrap bootstrap =
            new ExpeditionRuntimeBootstrap(
                sceneTransitionService,
                compositionContext);

        expeditionDirector =
            bootstrap.Build();

        ExpeditionRewardItemResolver
            expeditionRewardItemResolver =
        new ExpeditionRewardItemResolver(
            compositionContext.ExpeditionRewardItemDatabase);

        ItemInstanceFactory
            itemInstanceFactory =
                new ItemInstanceFactory();

        expeditionSettlementService =
            new ExpeditionSettlementService(
                context.PersistentInventoryRuntime.State.Items,
                context.PersistentInventoryRuntime.State.Materials,
                context.PersistentMetaRuntime.State,
                context.PersistentInventoryRuntime.State.SecureInventory,
                expeditionRewardItemResolver,
                itemInstanceFactory);

        expeditionSecurePreservationService =
            new ExpeditionSecurePreservationService(
                context.PersistentInventoryRuntime.State.Items,
                context.PersistentInventoryRuntime.State.SecureInventory);

        itemWorldDropService =
            bootstrap.BuildItemWorldDropService();

        itemWorldDropConfirmationService =
            new ItemWorldDropConfirmationService(
                context.GameFlow,
                itemWorldDropService);

        expeditionExitService =
            bootstrap.BuildExitService(
                expeditionDirector,
                expeditionSettlementService,
                expeditionSecurePreservationService,
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
            ExpeditionExitReason.Abandoned,
            CurrentRunConfig);
    }
}