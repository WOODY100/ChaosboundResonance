using System;
using UnityEngine;

[RequireComponent(typeof(EnemyRuntimeContext))]
[RequireComponent(typeof(EnemyHealth))]
public sealed class EnemyReward :
    MonoBehaviour
{
    [Header("Experience")]

    [SerializeField]
    private GameObject experienceFragmentPrefab;

    private EnemyRuntimeContext runtimeContext;
    private EnemyHealth health;

    private bool isInitialized;
    private bool rewardHandled;

    private void Awake()
    {
        runtimeContext =
            GetComponent<EnemyRuntimeContext>();

        health =
            GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        rewardHandled = false;

        if (health != null)
        {
            health.OnDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }

        rewardHandled = false;
        isInitialized = false;
    }

    /// <summary>
    /// Initializes the enemy reward runtime.
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

        if (health == null)
        {
            throw new InvalidOperationException(
                "EnemyHealth is not available.");
        }

        if (runtimeContext.Variant == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext does not contain an EnemyVariantData.");
        }

        if (runtimeContext.Variant.Rewards == null)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' does not contain an EnemyRewardDefinition.");
        }

        isInitialized = true;
        rewardHandled = false;
    }

    private void HandleDeath(
        EnemyHealth enemyHealth)
    {
        if (!isInitialized)
        {
            throw new InvalidOperationException(
                "EnemyReward has not been initialized.");
        }

        if (rewardHandled)
            return;

        rewardHandled = true;

        EnemyRewardDefinition definition =
            runtimeContext.Variant.Rewards;

        foreach (EnemyRewardEntryDefinition reward
            in definition.Rewards)
        {
            if (reward == null)
                continue;

            if (reward.Type != EnemyRewardType.Experience)
                continue;

            if (!RollChance(reward.Chance))
                continue;

            GrantExperience(
                reward.Amount);
        }
    }

    private void GrantExperience(
    int amount)
    {
        if (amount <= 0)
            return;

        if (experienceFragmentPrefab == null)
        {
            Debug.LogError(
                $"Enemy '{name}' cannot grant experience because " +
                "Experience Fragment Prefab is not assigned.",
                this);

            return;
        }

        if (PoolManager.Instance == null)
        {
            Debug.LogError(
                "EnemyReward could not find PoolManager.",
                this);

            return;
        }

        GameObject fragmentObject =
            PoolManager.Instance.Get(
                experienceFragmentPrefab,
                transform.position,
                experienceFragmentPrefab.transform.rotation);

        if (fragmentObject == null)
        {
            Debug.LogError(
                $"Enemy '{name}' failed to obtain an Experience Fragment from the pool.",
                this);

            return;
        }

        ResonanceFragmentPickup fragment =
            fragmentObject.GetComponent<ResonanceFragmentPickup>();

        if (fragment == null)
        {
            Debug.LogError(
                $"Experience Fragment prefab '{experienceFragmentPrefab.name}' " +
                "does not contain ResonanceFragmentPickup.",
                this);

            PooledObject pooledObject =
                fragmentObject.GetComponent<PooledObject>();

            pooledObject?.ReturnToPool();

            return;
        }

        fragment.Initialize(
            amount);

        runtimeContext
            .ExpeditionRuntime
            .XPFragments
            .Register(
                fragment);
    }

    private bool RollChance(
        float chance)
    {
        chance =
            Mathf.Clamp01(
                chance);

        if (chance <= 0f)
            return false;

        if (chance >= 1f)
            return true;

        return UnityEngine.Random.value <= chance;
    }
}