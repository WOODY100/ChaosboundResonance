using Chaosbound.Content.Portal.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal;
using Chaosbound.Gameplay.ExpeditionRuntime.References.Contracts;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    /// <summary>
    /// Materializes Exit Portal spawn tasks
    /// into the game world.
    /// </summary>
    public sealed class ExitPortalMaterializer :
        ISpawnMaterializer
    {
        private const string ExitPortalDomainId =
            "exitPortal";

        private readonly ISpawnInstantiationService
            instantiationService;

        public ExitPortalMaterializer(
            ISpawnInstantiationService instantiationService)
        {
            this.instantiationService =
                instantiationService
                ?? throw new ArgumentNullException(
                    nameof(instantiationService));
        }

        public GameObject Materialize(
            SpawnExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            if (context
                    .ResolvedTask
                    .ScheduledTask
                    .Task
                    .Entry
                    .Materializable
                    .Reference
                is not ExitPortalData exitPortal)
            {
                throw new InvalidOperationException(
                    "ExitPortalMaterializer received an unsupported materializable reference.");
            }

            SpawnPlacement placement =
                context
                    .ResolvedTask
                    .Placement
                    .Placement;

            SpawnInstantiationRequest request =
                new SpawnInstantiationRequest(
                    exitPortal,
                    placement.Position,
                    placement.Rotation);

            GameObject instance =
                instantiationService.Spawn(
                    request);

            if (instance == null)
            {
                throw new InvalidOperationException(
                    $"Exit Portal '{exitPortal.name}' could not be materialized.");
            }

            context
                .RuntimeState
                .ExpeditionRuntime
                .RuntimeReferences
                .Register(
                    ExitPortalDomainId,
                    exitPortal.Id,
                    instance.transform);

            ExitPortalInteractable interactable =
                instance.GetComponent<ExitPortalInteractable>();

            if (interactable == null)
            {
                throw new InvalidOperationException(
                    "Exit Portal prefab requires an " +
                    "ExitPortalInteractable component.");
            }

            interactable.Initialize(
                context.RuntimeState
                    .ExpeditionRuntime
                    .ExitPortal);

            return instance;
        }
    }
}