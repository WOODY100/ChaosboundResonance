using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Synchronizes Boss death with Expedition completion and delays pool
    /// return until the visual death presentation has completed.
    /// </summary>
    [RequireComponent(typeof(BossHealth))]
    [RequireComponent(typeof(BossRuntimeContext))]
    [RequireComponent(typeof(BossRuntimePresentation))]
    public sealed class BossRuntimeLifecycle : MonoBehaviour
    {
        private BossHealth health;
        private BossRuntimeContext runtimeContext;
        private BossRuntimePresentation presentation;

        private Action<GameObject> deathPresentationCompletion;
        private bool deathHandled;
        private bool completionReported;
        private bool deathPresentationCompleted;
        private bool returnRequested;

        private void Awake()
        {
            CacheReferences();
        }

        private void OnEnable()
        {
            CacheReferences();

            if (health != null)
                health.OnDeath += HandleDeath;

            if (presentation != null)
                presentation.DeathPresentationCompleted +=
                    HandleDeathPresentationCompleted;

            deathHandled = false;
            completionReported = false;
            deathPresentationCompleted = false;
            returnRequested = false;
        }

        /// <summary>
        /// Binds the owning encounter's SpawnRuntime release operation.
        /// The encounter orchestrator must call this before StartCombat.
        /// </summary>
        public void BindDeathPresentationCompletion(
            Action<GameObject> completion)
        {
            if (completion == null)
                throw new ArgumentNullException(nameof(completion));

            if (deathHandled)
                throw new InvalidOperationException(
                    "Cannot bind a Boss lifecycle after death has been handled.");

            deathPresentationCompletion = completion;
        }

        private void HandleDeath(BossHealth bossHealth)
        {
            if (deathHandled)
                return;

            if (runtimeContext == null || !runtimeContext.IsInitialized)
            {
                Debug.LogError(
                    "BossRuntimeLifecycle received death before BossRuntimeContext initialization.",
                    this);
                return;
            }

            deathHandled = true;
            ReportCompletion();

            // Presentation and Health are independent event subscribers.
            // The animation can finish before this handler is invoked.
            if (deathPresentationCompleted)
                RequestReturnToPool();
        }

        private void ReportCompletion()
        {
            if (completionReported)
                return;

            string domainId;
            string eventId;

            if (runtimeContext.Boss != null)
            {
                domainId = "boss";
                eventId = "boss";
                runtimeContext.ExpeditionRuntime.Boss.Complete();
            }
            else if (runtimeContext.MiniBoss != null)
            {
                domainId = "miniboss";
                eventId = "miniboss";
                runtimeContext.ExpeditionRuntime.MiniBoss.Complete();
            }
            else
            {
                Debug.LogError(
                    "BossRuntimeContext has no supported Boss or MiniBoss content.",
                    this);
                return;
            }

            CompletionOrigin origin = new CompletionOrigin(
                domainId,
                runtimeContext.Content.Id);

            runtimeContext.ExpeditionRuntime.ReportEventCompleted(
                new EventCompleted(domainId, eventId, origin));

            completionReported = true;
        }

        private void HandleDeathPresentationCompleted()
        {
            if (deathPresentationCompleted)
                return;

            deathPresentationCompleted = true;

            // Health and Presentation subscribe independently; do not depend
            // on Unity invoking their event handlers in a particular order.
            if (deathHandled)
                RequestReturnToPool();
        }

        private void RequestReturnToPool()
        {
            if (returnRequested)
                return;

            if (deathPresentationCompletion == null)
            {
                Debug.LogError(
                    "Boss death presentation completed without an encounter release binding. " +
                    "The Boss will remain materialized.",
                    this);
                return;
            }

            returnRequested = true;
            deathPresentationCompletion(gameObject);
        }

        private void CacheReferences()
        {
            if (health == null)
                health = GetComponent<BossHealth>();
            if (runtimeContext == null)
                runtimeContext = GetComponent<BossRuntimeContext>();
            if (presentation == null)
                presentation = GetComponent<BossRuntimePresentation>();
        }

        private void OnDisable()
        {
            if (health != null)
                health.OnDeath -= HandleDeath;

            if (presentation != null)
                presentation.DeathPresentationCompleted -=
                    HandleDeathPresentationCompleted;

            deathPresentationCompletion = null;
            deathHandled = false;
            completionReported = false;
            deathPresentationCompleted = false;
            returnRequested = false;
        }
    }
}
