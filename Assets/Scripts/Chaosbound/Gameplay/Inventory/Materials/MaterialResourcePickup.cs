using Chaosbound.Content.Materials;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using UnityEngine;

public sealed class MaterialResourcePickup :
    AutoPickupBehaviour,
    IResourcePickup
{
    [Header("Material")]
    [SerializeField]
    private string contentId;

    private int amount;
    private bool isInitialized;

    private MaterialResolver materialResolver;

    public string ContentId =>
        contentId;

    public int Amount =>
        amount;

    protected override float GetPickupRadius()
    {
        return GetPlayerPickupRadius();
    }

    protected override void OnAutoPickupTriggered()
    {
        if (!isInitialized)
            return;

        if (string.IsNullOrEmpty(contentId))
            return;

        if (amount <= 0)
            return;

        if (RunManager.Instance == null)
            return;

        ExpeditionRuntimeState runtimeState =
            RunManager.Instance.ExpeditionRuntimeState;

        if (runtimeState == null)
            return;

        if (!TryResolveMaterial(
                out MaterialDefinition materialDefinition))
        {
            return;
        }

        runtimeState.Materials.Add(
            materialDefinition.ContentId,
            amount);

        ReturnToPool();
    }

    public void Initialize(int amount)
    {
        if (amount <= 0)
            throw new System.ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Material amount must be greater than zero.");

        ResetPooledState();

        this.amount = amount;
        isInitialized = true;
    }

    protected override void ResetPooledState()
    {
        base.ResetPooledState();

        amount = 0;
        isInitialized = false;
    }

    private bool TryResolveMaterial(
        out MaterialDefinition materialDefinition)
    {
        materialDefinition = null;

        GameContentContext context =
            GameContentContext.Current;

        if (context == null)
            return false;

        if (context.MaterialDatabase == null)
            return false;

        if (materialResolver == null)
        {
            materialResolver =
                new MaterialResolver(
                    context.MaterialDatabase);
        }

        return materialResolver.TryResolve(
            contentId,
            out materialDefinition);
    }
}