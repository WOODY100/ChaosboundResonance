using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Shared.Identifiers;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Factories
{
    /// <summary>
    /// Builds the shared runtime context used by the Expedition Runtime.
    /// The Player is registered once in the expedition reference registry.
    /// </summary>
    public sealed class ExpeditionRuntimeContextFactory
    {
        private const string PlayerDomainId = "player";
        private static readonly ContentId PlayerContentId = new ContentId("player");

        private readonly RuntimeExpeditionConfig runtimeConfig;
        private readonly ExpeditionRuntimeState runtimeState;

        public ExpeditionRuntimeContext Create()
        {
            ExpeditionRuntimeReferences references = BuildReferences();
            ExpeditionRuntimeServices services = BuildServices();

            return new ExpeditionRuntimeContext(
                runtimeConfig,
                references,
                services,
                runtimeState);
        }

        private ExpeditionRuntimeReferences BuildReferences()
        {
            RuntimeReferencesConfig runtime = BuildRuntimeReferences();
            return new ExpeditionRuntimeReferences(runtime);
        }

        private RuntimeReferencesConfig BuildRuntimeReferences()
        {
            ExpeditionSceneContext scene = ExpeditionSceneContext.Current;

            if (scene == null)
                throw new InvalidOperationException(
                    "ExpeditionSceneContext is not available.");

            if (scene.Player == null)
                throw new InvalidOperationException(
                    "ExpeditionSceneContext does not contain a Player.");

            Transform playerTransform = scene.Player.transform;

            runtimeState.RuntimeReferences.Register(
                PlayerDomainId,
                PlayerContentId,
                playerTransform);

            return new RuntimeReferencesConfig(playerTransform);
        }

        private ExpeditionRuntimeServices BuildServices()
        {
            return new ExpeditionRuntimeServices();
        }

        public ExpeditionRuntimeContextFactory(
            RuntimeExpeditionConfig runtimeConfig,
            ExpeditionRuntimeState runtimeState)
        {
            this.runtimeConfig = runtimeConfig
                ?? throw new ArgumentNullException(nameof(runtimeConfig));
            this.runtimeState = runtimeState
                ?? throw new ArgumentNullException(nameof(runtimeState));
        }
    }
}
