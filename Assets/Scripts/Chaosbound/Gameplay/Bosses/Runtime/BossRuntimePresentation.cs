using Chaosbound.Content.Enemy.Bosses;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Visual-only bridge between Boss Runtime and a character Animator.
    /// It never advances combat, releases attacks, applies damage or moves the Boss.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BossCombatRuntime))]
    [RequireComponent(typeof(BossMovementRuntime))]
    [RequireComponent(typeof(BossHealth))]
    public sealed class BossRuntimePresentation : MonoBehaviour
    {
        [Serializable]
        private sealed class AttackAnimationBinding
        {
            [SerializeField] private string m_AttackContentId;
            [SerializeField] private string m_AnimatorState;

            public string AttackContentId => m_AttackContentId;
            public string AnimatorState => m_AnimatorState;
        }

        [Header("References")]
        [SerializeField] private Animator visualAnimator;

        [Header("Locomotion States")]
        [SerializeField] private string idleState = "Idle";
        [SerializeField] private string walkState = "Walk";
        [SerializeField] private string retreatState = "Walk_Back";
        [SerializeField] private string deathState = "Death";

        [Header("Attack States")]
        [SerializeField] private List<AttackAnimationBinding> attackAnimations =
            new List<AttackAnimationBinding>();

        [Header("Playback")]
        [SerializeField, Min(0f)] private float crossFadeDuration = 0.08f;
        [SerializeField] private bool playIdleOnEnable = true;

        [Header("Death Presentation")]
        [SerializeField, Min(0.1f)]
        private float deathCompletionTimeout = 8f;

        private BossCombatRuntime combatRuntime;
        private BossMovementRuntime movementRuntime;
        private BossHealth health;
        private readonly HashSet<int> warnedMissingStates = new HashSet<int>();

        private int currentStateHash;
        private bool hasCurrentState;
        private bool attackPresentationActive;
        private bool isDead;
        private bool deathPresentationPending;
        private float deathPresentationElapsed;

        /// <summary>
        /// Raised after the Death Animator state completes, or after the
        /// configured watchdog expires if the Animator cannot complete it.
        /// This event is presentation lifecycle only; it does not affect combat.
        /// </summary>
        public event Action DeathPresentationCompleted;

        private void Awake()
        {
            CacheReferences();
        }

        private void OnEnable()
        {
            CacheReferences();

            if (combatRuntime != null)
                combatRuntime.PresentationStateChanged += HandlePresentationStateChanged;

            if (health != null)
                health.OnDeath += HandleDeath;

            attackPresentationActive = false;
            deathPresentationPending = false;
            deathPresentationElapsed = 0f;
            // BossHealth also resets pooled health during OnEnable. Do not infer
            // death from its previous pooled value here; death is driven by
            // the explicit OnDeath event after runtime initialization.
            isDead = false;

            if (playIdleOnEnable)
                PlayState(idleState);
        }

        private void Update()
        {
            if (deathPresentationPending)
            {
                TickDeathPresentation();
                return;
            }

            if (isDead || attackPresentationActive || visualAnimator == null ||
                movementRuntime == null)
                return;

            BossMovementState movementState = movementRuntime.State;

            switch (movementState)
            {
                case BossMovementState.Approach:
                    PlayState(walkState);
                    break;

                case BossMovementState.Retreat:
                    // Use the dedicated backward locomotion state when configured.
                    // Fall back to the regular walk state for Bosses without one.
                    PlayState(
                        string.IsNullOrWhiteSpace(retreatState)
                            ? walkState
                            : retreatState);
                    break;

                default:
                    PlayState(idleState);
                    break;
            }
        }

        private void HandlePresentationStateChanged(
            BossAttackDefinition attack,
            BossAttackExecutionState state)
        {
            if (isDead || attack == null)
                return;

            switch (state)
            {
                case BossAttackExecutionState.Anticipation:
                    attackPresentationActive = true;
                    PlayAttackState(attack);
                    break;

                case BossAttackExecutionState.Execution:
                    attackPresentationActive = true;
                    break;

                case BossAttackExecutionState.Recovery:
                    attackPresentationActive = true;
                    if (!string.IsNullOrWhiteSpace(attack.Recovery.PresentationVariant))
                        PlayState(attack.Recovery.PresentationVariant, true);
                    break;

                case BossAttackExecutionState.Completed:
                case BossAttackExecutionState.Cancelled:
                case BossAttackExecutionState.Failed:
                    attackPresentationActive = false;
                    break;
            }
        }

        private void PlayAttackState(BossAttackDefinition attack)
        {
            string stateName = null;

            if (!string.IsNullOrWhiteSpace(attack.Anticipation.PresentationVariant))
                stateName = attack.Anticipation.PresentationVariant;
            else
                stateName = FindAttackState(attack.ContentId);

            if (string.IsNullOrWhiteSpace(stateName))
            {
                Debug.LogWarning(
                    "BossRuntimePresentation has no Animator state mapping for attack '" +
                    attack.ContentId + "'.", this);
                return;
            }

            PlayState(stateName, true);
        }

        private string FindAttackState(string attackContentId)
        {
            if (attackAnimations == null)
                return null;

            for (int i = 0; i < attackAnimations.Count; i++)
            {
                AttackAnimationBinding binding = attackAnimations[i];
                if (binding == null ||
                    !string.Equals(
                        binding.AttackContentId,
                        attackContentId,
                        StringComparison.Ordinal))
                    continue;

                return binding.AnimatorState;
            }

            return null;
        }

        private void HandleDeath(BossHealth source)
        {
            if (isDead)
                return;

            BeginDeathPresentation();
        }

        private void BeginDeathPresentation()
        {
            isDead = true;
            attackPresentationActive = false;
            deathPresentationElapsed = 0f;

            if (visualAnimator == null ||
                string.IsNullOrWhiteSpace(deathState) ||
                !visualAnimator.HasState(0, Animator.StringToHash(deathState)))
            {
                if (visualAnimator != null && !string.IsNullOrWhiteSpace(deathState))
                {
                    Debug.LogWarning(
                        "BossRuntimePresentation cannot play Death state '" +
                        deathState + "'. Completing death presentation immediately.",
                        this);
                }

                CompleteDeathPresentation();
                return;
            }

            deathPresentationPending = true;
            PlayState(deathState, true);
        }

        private void TickDeathPresentation()
        {
            deathPresentationElapsed += Time.deltaTime;

            if (visualAnimator != null && !visualAnimator.IsInTransition(0))
            {
                AnimatorStateInfo stateInfo =
                    visualAnimator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName(deathState) &&
                    stateInfo.normalizedTime >= 1f)
                {
                    CompleteDeathPresentation();
                    return;
                }
            }

            if (deathPresentationElapsed >= deathCompletionTimeout)
            {
                Debug.LogWarning(
                    "BossRuntimePresentation reached its Death presentation timeout. " +
                    "The Boss will be returned to its pool to avoid a stuck encounter.",
                    this);
                CompleteDeathPresentation();
            }
        }

        private void CompleteDeathPresentation()
        {
            if (!deathPresentationPending && !isDead)
                return;

            deathPresentationPending = false;
            DeathPresentationCompleted?.Invoke();
        }

        private void PlayState(string stateName, bool forceRestart = false)
        {
            if (visualAnimator == null || string.IsNullOrWhiteSpace(stateName))
                return;

            int stateHash = Animator.StringToHash(stateName);
            bool isAlreadyCurrent = hasCurrentState && currentStateHash == stateHash;

            if (isAlreadyCurrent && !forceRestart)
                return;

            if (!visualAnimator.HasState(0, stateHash))
            {
                if (warnedMissingStates.Add(stateHash))
                {
                    Debug.LogWarning(
                        "Animator '" + visualAnimator.name +
                        "' does not contain state '" + stateName + "' on layer 0.",
                        visualAnimator);
                }
                return;
            }

            if (isAlreadyCurrent && forceRestart)
            {
                visualAnimator.Play(stateHash, 0, 0f);
            }
            else
            {
                visualAnimator.CrossFadeInFixedTime(
                    stateHash,
                    crossFadeDuration,
                    0,
                    0f);
            }

            currentStateHash = stateHash;
            hasCurrentState = true;
        }

        private void CacheReferences()
        {
            if (combatRuntime == null)
                combatRuntime = GetComponent<BossCombatRuntime>();
            if (movementRuntime == null)
                movementRuntime = GetComponent<BossMovementRuntime>();
            if (health == null)
                health = GetComponent<BossHealth>();
            if (visualAnimator == null)
                visualAnimator = GetComponentInChildren<Animator>(true);
        }

        private void OnDisable()
        {
            if (combatRuntime != null)
                combatRuntime.PresentationStateChanged -= HandlePresentationStateChanged;

            if (health != null)
                health.OnDeath -= HandleDeath;

            attackPresentationActive = false;
            isDead = false;
            deathPresentationPending = false;
            deathPresentationElapsed = 0f;
            hasCurrentState = false;
            warnedMissingStates.Clear();
        }

        private void OnValidate()
        {
            crossFadeDuration = Mathf.Max(0f, crossFadeDuration);
            deathCompletionTimeout = Mathf.Max(0.1f, deathCompletionTimeout);
        }
    }
}
