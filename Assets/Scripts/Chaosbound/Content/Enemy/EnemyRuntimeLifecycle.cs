using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyRuntimeContext))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyCombat))]
[RequireComponent(typeof(EnemyRuntimeBehavior))]
[RequireComponent(typeof(EnemyRuntimeBehaviorScheduler))]
[RequireComponent(typeof(EnemyRuntimeNavigation))]
[RequireComponent(typeof(EnemyRuntimePresentation))]
[RequireComponent(typeof(PooledObject))]
public sealed class EnemyRuntimeLifecycle :
    MonoBehaviour
{
    private EnemyRuntimeContext runtimeContext;
    private EnemyHealth health;
    private EnemyCombat combat;
    private EnemyRuntimeBehavior behavior;
    private EnemyRuntimeBehaviorScheduler scheduler;
    private EnemyRuntimeNavigation navigation;
    private EnemyRuntimePresentation presentation;
    private PooledObject pooledObject;
    private Collider enemyCollider;

    private Coroutine deathRoutine;

    private bool deathHandled;

    private void Awake()
    {
        runtimeContext =
            GetComponent<EnemyRuntimeContext>();

        health =
            GetComponent<EnemyHealth>();

        combat =
            GetComponent<EnemyCombat>();

        behavior =
            GetComponent<EnemyRuntimeBehavior>();

        scheduler =
            GetComponent<EnemyRuntimeBehaviorScheduler>();

        navigation =
            GetComponent<EnemyRuntimeNavigation>();

        presentation =
            GetComponent<EnemyRuntimePresentation>();

        pooledObject =
            GetComponent<PooledObject>();
        
        enemyCollider =
            GetComponent<Collider>();
    }

    private void OnEnable()
    {
        deathHandled = false;

        if (enemyCollider != null)
        {
            enemyCollider.enabled = true;
        }

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

        StopDeathRoutine();

        deathHandled = false;
    }

    private void HandleDeath(
        EnemyHealth enemyHealth)
    {
        if (deathHandled)
            return;

        deathHandled = true;

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

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

        if (presentation == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimePresentation is not available.");
        }

        if (!presentation.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimePresentation has not been initialized.");
        }

        if (presentation.DeathClip == null)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' does not have a Death Clip.");
        }

        if (pooledObject == null)
        {
            throw new InvalidOperationException(
                "PooledObject is not available.");
        }

        scheduler.Shutdown();

        behavior.Shutdown();

        combat.Shutdown();

        navigation.Shutdown();

        presentation.PlayDeath(
            presentation.DeathClip);

        runtimeContext
            .ExpeditionRuntime
            .RuntimeComposition
            .Decrement(
                runtimeContext.Variant);

        deathRoutine =
            StartCoroutine(
                DeathRoutine(
                    presentation.DeathClip.length));
    }

    private IEnumerator DeathRoutine(
        float duration)
    {
        yield return new WaitForSeconds(
            duration);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (pooledObject == null)
        {
            pooledObject =
                GetComponent<PooledObject>();
        }

        if (pooledObject != null)
        {
            pooledObject.ReturnToPool();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void StopDeathRoutine()
    {
        if (deathRoutine == null)
            return;

        StopCoroutine(
            deathRoutine);

        deathRoutine = null;
    }
}