using Chaosbound.Content.Expeditions.Enums.Spawn;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Content.Portal.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Integration.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Models;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Services;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal
{
    /// <summary>
    /// Coordinates Exit Portal Domain runtime behavior
    /// for the current expedition.
    /// </summary>
    public sealed class ExitPortalDomainDirector
    {
        private readonly ExitPortalSpawnPlanner
            spawnPlanner;

        private readonly ExitPortalSpawnRequestTranslator
            spawnRequestTranslator;

        private readonly SpawnRuntime
            spawnRuntime;

        public ExitPortalDomainDirector(
            ExitPortalSpawnPlanner spawnPlanner,
            ExitPortalSpawnRequestTranslator spawnRequestTranslator,
            SpawnRuntime spawnRuntime)
        {
            this.spawnPlanner =
                spawnPlanner
                ?? throw new ArgumentNullException(
                    nameof(spawnPlanner));

            this.spawnRequestTranslator =
                spawnRequestTranslator
                ?? throw new ArgumentNullException(
                    nameof(spawnRequestTranslator));

            this.spawnRuntime =
                spawnRuntime
                ?? throw new ArgumentNullException(
                    nameof(spawnRuntime));
        }

        /// <summary>
        /// Executes Exit Portal Domain behavior for
        /// the current expedition runtime tick.
        /// </summary>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            ExitPortalRuntimeState state =
                context.State.ExitPortal;

            if (state.State ==
                ExitPortalDomainState.Spawned)
            {
                return;
            }

            if (context.State.Completion.State !=
                CompletionDomainState.Completed)
            {
                return;
            }

            ExitPortalData exitPortal =
                context.Config.Completion.ExitPortal;

            if (exitPortal == null)
            {
                throw new InvalidOperationException(
                    "The current expedition does not define " +
                    "an Exit Portal for Completion.");
            }

            if (state.State ==
                ExitPortalDomainState.Inactive)
            {
                state.Start();
            }

            ExitPortalSpawnPlan spawnPlan =
                spawnPlanner.Build(
                    exitPortal);

            SpawnRequest spawnRequest =
                spawnRequestTranslator.Translate(
                    spawnPlan,
                    context.Config.Spawn);

            RuntimeSpawnConfig portalSpawnConfig =
                context.Config.Spawn.WithPlacement(
                    SpawnPlacementPolicy.AroundCompletionOrigin);

            spawnRuntime.Execute(
                spawnRequest,
                portalSpawnConfig,
                context.References.Runtime,
                context.State);

            state.MarkSpawned();
        }

        /// <summary>
        /// Consumes a pending player interaction with the
        /// Exit Portal and determines whether the expedition
        /// can be exited.
        /// </summary>
        public bool TryConsumeExitRequest(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            ExitPortalRuntimeState state =
                context.State.ExitPortal;

            if (!state.InteractionRequested)
                return false;

            if (state.State !=
                ExitPortalDomainState.Spawned)
            {
                state.ClearInteractionRequest();

                return false;
            }

            if (context.State.Completion.State !=
                CompletionDomainState.Completed)
            {
                state.ClearInteractionRequest();

                return false;
            }

            state.ClearInteractionRequest();

            Debug.Log(
                "[ExitPortalDomain] Exit request accepted.");

            return true;
        }
    }
}