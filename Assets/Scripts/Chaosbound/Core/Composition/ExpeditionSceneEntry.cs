using System;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using UnityEngine;

namespace Chaosbound.Core.Composition
{
    public sealed class ExpeditionSceneEntry : MonoBehaviour
    {
        [Header("Scene Context")]
        [SerializeField]
        private ExpeditionSceneContext sceneContext;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            Validate();

            BootstrapContext bootstrap = BootstrapContext.Current;
            RunSession runSession = bootstrap.RunSession;
            RuntimeExpeditionConfig runtimeConfig = runSession.CurrentRun;

            ExpeditionComposition composition =
                CreateComposition(
                    bootstrap,
                    runtimeConfig);

            composition.Initialize();
        }

        private ExpeditionComposition CreateComposition(
            BootstrapContext bootstrap,
            RuntimeExpeditionConfig runtimeConfig)
        {
            return new ExpeditionComposition(
                bootstrap,
                runtimeConfig,
                sceneContext);
        }

        private void Validate()
        {
            BootstrapContext bootstrap = BootstrapContext.Current;

            if (bootstrap == null)
            {
                throw new InvalidOperationException(
                    "BootstrapContext was not found.");
            }

            if (sceneContext == null)
            {
                throw new InvalidOperationException(
                    "ExpeditionSceneContext reference is missing.");
            }

            RunSession runSession = bootstrap.RunSession;

            if (runSession == null)
            {
                throw new InvalidOperationException(
                    "RunSession reference is missing.");
            }

            if (!runSession.HasRun)
            {
                throw new InvalidOperationException(
                    "There is no active RunSession.");
            }
        }
    }
}