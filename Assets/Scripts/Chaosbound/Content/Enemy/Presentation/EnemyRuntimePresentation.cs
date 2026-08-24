using System;
using UnityEngine;

[RequireComponent(typeof(EnemyRuntimeContext))]
[RequireComponent(typeof(EnemyRuntimeNavigation))]
public sealed class EnemyRuntimePresentation :
    MonoBehaviour
{
    [Header("Attack Presentation")]

    [SerializeField]
    private string attackLayerName = "Attack Layer";

    [SerializeField]
    private string attackStateName = "Attack";

    [SerializeField]
    private AnimationClip attackBaseClip;

    [Header("Death Presentation")]

    [SerializeField]
    private string deathLayerName = "Death Layer";

    [SerializeField]
    private string deathStateName = "Death";

    [SerializeField]
    private AnimationClip deathBaseClip;

    private EnemyRuntimeContext runtimeContext;
    private EnemyRuntimeNavigation navigation;
    private Animator animator;

    private AnimatorOverrideController
        animatorOverrideController;

    private int attackLayerIndex;
    private int attackStateHash;

    private int deathLayerIndex;
    private int deathStateHash;

    private static readonly int
        SpeedHash =
            Animator.StringToHash("Speed");

    public bool IsInitialized
    {
        get;
        private set;
    }

    public float CurrentSpeed
    {
        get;
        private set;
    }

    public bool IsPlayingDeath
    {
        get;
        private set;
    }

    public AnimationClip DeathClip =>
        deathBaseClip;

    private void Awake()
    {
        runtimeContext =
            GetComponent<EnemyRuntimeContext>();

        navigation =
            GetComponent<EnemyRuntimeNavigation>();

        animator =
            GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Initializes enemy presentation.
    /// </summary>
    public void Initialize()
    {
        if (runtimeContext == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext is not available.");
        }

        if (!runtimeContext.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext has not been initialized.");
        }

        if (navigation == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeNavigation is not available.");
        }

        if (animator == null)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' does not contain an Animator.");
        }

        if (animator.runtimeAnimatorController == null)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' Animator does not contain " +
                "a RuntimeAnimatorController.");
        }

        if (attackBaseClip == null)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' presentation does not contain " +
                "an Attack Base Clip.");
        }

        if (deathBaseClip == null)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' presentation does not contain " +
                "a Death Base Clip.");
        }

        attackLayerIndex =
            animator.GetLayerIndex(
                attackLayerName);

        if (attackLayerIndex < 0)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' Animator does not contain " +
                $"an '{attackLayerName}' layer.");
        }

        deathLayerIndex =
            animator.GetLayerIndex(
                deathLayerName);

        if (deathLayerIndex < 0)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' Animator does not contain " +
                $"a '{deathLayerName}' layer.");
        }

        attackStateHash =
            Animator.StringToHash(
                attackStateName);

        deathStateHash =
            Animator.StringToHash(
                deathStateName);

        animatorOverrideController =
            new AnimatorOverrideController(
                animator.runtimeAnimatorController);

        animator.runtimeAnimatorController =
            animatorOverrideController;

        animatorOverrideController[
            attackBaseClip.name] =
            attackBaseClip;

        animatorOverrideController[
            deathBaseClip.name] =
            deathBaseClip;

        animator.SetLayerWeight(
            attackLayerIndex,
            0f);

        animator.SetLayerWeight(
            deathLayerIndex,
            0f);

        animator.applyRootMotion = false;

        CurrentSpeed = 0f;

        IsPlayingDeath = false;

        animator.SetFloat(
            SpeedHash,
            0f);

        IsInitialized = true;
    }

    private void Update()
    {
        if (!IsInitialized)
            return;

        UpdateLocomotion();
        UpdateFacing();
    }

    /// <summary>
    /// Requests the presentation of an enemy attack.
    /// Combat remains the authority over attack timing.
    /// </summary>
    public void PlayAttack(
        AnimationClip clip)
    {
        ValidateInitialized();

        if (IsPlayingDeath)
            return;

        if (clip == null)
        {
            throw new ArgumentNullException(
                nameof(clip));
        }

        animatorOverrideController[
            attackBaseClip.name] =
            clip;

        animator.SetLayerWeight(
            deathLayerIndex,
            0f);

        animator.SetLayerWeight(
            attackLayerIndex,
            1f);

        animator.Play(
            attackStateHash,
            attackLayerIndex,
            0f);
    }

    /// <summary>
    /// Stops the current attack presentation.
    /// Does not modify combat state.
    /// </summary>
    public void StopAttack()
    {
        ValidateInitialized();

        animator.SetLayerWeight(
            attackLayerIndex,
            0f);
    }

    /// <summary>
    /// Requests the presentation of enemy death.
    /// Lifecycle remains the authority over death timing
    /// and pool return.
    /// </summary>
    public void PlayDeath(
        AnimationClip clip)
    {
        ValidateInitialized();

        if (clip == null)
        {
            throw new ArgumentNullException(
                nameof(clip));
        }

        IsPlayingDeath = true;

        animator.SetLayerWeight(
            attackLayerIndex,
            0f);

        animatorOverrideController[
            deathBaseClip.name] =
            clip;

        animator.SetLayerWeight(
            deathLayerIndex,
            1f);

        animator.Play(
            deathStateHash,
            deathLayerIndex,
            0f);
    }

    /// <summary>
    /// Updates locomotion presentation from the actual
    /// NavMeshAgent velocity.
    /// </summary>
    private void UpdateLocomotion()
    {
        Vector3 velocity =
            navigation.Velocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z);

        float speed =
            horizontalVelocity.magnitude;

        CurrentSpeed =
            speed;

        animator.SetFloat(
            SpeedHash,
            speed);
    }

    /// <summary>
    /// Makes the enemy face its actual movement direction.
    /// </summary>
    private void UpdateFacing()
    {
        Vector3 velocity =
            navigation.Velocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z);

        if (horizontalVelocity.sqrMagnitude <=
            0.0001f)
        {
            return;
        }

        transform.rotation =
            Quaternion.LookRotation(
                horizontalVelocity,
                Vector3.up);
    }

    /// <summary>
    /// Resets presentation state so a pooled enemy
    /// never retains runtime presentation data.
    /// </summary>
    public void ResetPresentation()
    {
        if (animator != null)
        {
            if (attackLayerIndex >= 0)
            {
                animator.SetLayerWeight(
                    attackLayerIndex,
                    0f);
            }

            if (deathLayerIndex >= 0)
            {
                animator.SetLayerWeight(
                    deathLayerIndex,
                    0f);
            }
        }

        CurrentSpeed = 0f;
        IsPlayingDeath = false;
    }

    private void ValidateInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimePresentation has not been initialized.");
        }

        if (animator == null)
        {
            throw new InvalidOperationException(
                "Animator is not available.");
        }

        if (animatorOverrideController == null)
        {
            throw new InvalidOperationException(
                "AnimatorOverrideController is not available.");
        }
    }

    private void OnDisable()
    {
        ResetPresentation();

        IsInitialized = false;
    }
}