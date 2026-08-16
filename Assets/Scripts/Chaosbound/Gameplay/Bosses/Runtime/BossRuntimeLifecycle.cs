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
        private BossHealth health;

        private BossRuntimeContext runtimeContext;

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
        }

        private void HandleDeath(
            BossHealth bossHealth)
        {
            if (!runtimeContext.IsInitialized)
            {
                throw new InvalidOperationException(
                    "BossRuntimeContext has not been initialized.");
            }

            runtimeContext
                .ExpeditionRuntime
                .Boss
                .Complete();
        }
    }
}