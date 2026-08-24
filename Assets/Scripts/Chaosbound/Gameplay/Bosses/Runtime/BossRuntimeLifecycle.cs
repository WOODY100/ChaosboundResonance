using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Synchronizes the Boss entity lifecycle
    /// with the Boss Domain runtime state.
    /// </summary>
    [RequireComponent(typeof(BossHealth))]
    [RequireComponent(typeof(BossRuntimeContext))]
    public sealed class BossRuntimeLifecycle :
        MonoBehaviour
    {
        private const string BossDomainId =
            "boss";

        private const string BossCompletionEventId =
            "boss";

        private BossHealth health;

        private BossRuntimeContext runtimeContext;

        private bool completionReported;

        private void Awake()
        {
            health =
                GetComponent<BossHealth>();

            runtimeContext =
                GetComponent<BossRuntimeContext>();
        }

        private void OnEnable()
        {
            health.OnDeath +=
                HandleDeath;
        }

        private void OnDisable()
        {
            health.OnDeath -=
                HandleDeath;

            completionReported = false;
        }

        private void HandleDeath(
            BossHealth bossHealth)
        {
            if (!runtimeContext.IsInitialized)
            {
                throw new InvalidOperationException(
                    "BossRuntimeContext has not been initialized.");
            }

            if (completionReported)
            {
                return;
            }

            runtimeContext
                .ExpeditionRuntime
                .Boss
                .Complete();

            CompletionOrigin origin =
                new CompletionOrigin(
                    BossDomainId,
                    runtimeContext.Boss.Id);

            runtimeContext
                .ExpeditionRuntime
                .ReportEventCompleted(
                    new EventCompleted(
                        BossDomainId,
                        BossCompletionEventId,
                        origin));

            completionReported = true;
        }
    }
}