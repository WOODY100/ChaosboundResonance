using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PooledObject))]
[RequireComponent(typeof(LootBagRuntimeContext))]
public sealed class LootBag : PooledBehaviour
{
    [Header("Loot")]

    [SerializeField]
    private LootDefinition lootDefinition;

    [Header("Opening")]

    [SerializeField, Min(0f)]
    private float openDelay = 2f;

    private Coroutine openRoutine;
    private bool isOpened;
    private bool isInitialized;

    private LootBagRuntimeContext runtimeContext;

    public LootDefinition LootDefinition =>
        lootDefinition;

    public bool IsOpened =>
        isOpened;

    public float OpenDelay =>
        openDelay;

    protected override void Awake()
    {
        base.Awake();

        runtimeContext =
            GetComponent<LootBagRuntimeContext>();
    }

    protected override void ResetPooledState()
    {
        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
            openRoutine = null;
        }

        isOpened = false;
        isInitialized = false;
    }

    public void Initialize()
    {
        if (isInitialized)
            return;

        if (lootDefinition == null)
        {
            Debug.LogError(
                $"Loot Bag '{name}' cannot initialize because " +
                "Loot Definition is not assigned.",
                this);

            ReturnToPool();
            return;
        }

        isInitialized = true;
        isOpened = false;

        openRoutine =
            StartCoroutine(
                OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        if (openDelay > 0f)
        {
            yield return new WaitForSeconds(
                openDelay);
        }

        openRoutine = null;

        Open();
    }

    public void Open()
    {
        if (!isInitialized)
            return;

        if (isOpened)
            return;

        if (runtimeContext == null)
        {
            Debug.LogError(
                $"Loot Bag '{name}' cannot open because " +
                "LootBagRuntimeContext is not available.",
                this);

            return;
        }

        if (!runtimeContext.IsInitialized)
        {
            Debug.LogError(
                $"Loot Bag '{name}' cannot open because " +
                "LootBagRuntimeContext has not been initialized.",
                this);

            return;
        }

        if (runtimeContext.ExpeditionRuntime == null)
        {
            Debug.LogError(
                $"Loot Bag '{name}' cannot open because " +
                "ExpeditionRuntimeState is not available.",
                this);

            return;
        }

        isOpened = true;

        PendingLoot pendingLoot =
            new PendingLoot(
                lootDefinition,
                transform.position);

        runtimeContext
            .ExpeditionRuntime
            .Reward
            .AddPendingLoot(
                pendingLoot);

        Debug.Log(
            $"Loot Bag '{name}' opened.",
            this);

        ReturnToPool();
    }

    protected override void OnDisable()
    {
        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
            openRoutine = null;
        }

        base.OnDisable();
    }
}