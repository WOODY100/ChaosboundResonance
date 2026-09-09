using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using UnityEngine;

public sealed class LootBagRuntimeContext :
    MonoBehaviour
{
    public ExpeditionRuntimeState
        ExpeditionRuntime
    {
        get;
        private set;
    }

    public bool IsInitialized
    {
        get;
        private set;
    }

    public void Initialize(
        ExpeditionRuntimeState expeditionRuntime)
    {
        ExpeditionRuntime =
            expeditionRuntime
            ?? throw new ArgumentNullException(
                nameof(expeditionRuntime));

        ExpeditionRuntime.LootBags.Register(
            GetComponent<LootBag>());

        IsInitialized = true;
    }

    private void OnDisable()
    {
        if (ExpeditionRuntime != null)
        {
            ExpeditionRuntime.LootBags.Unregister(
                GetComponent<LootBag>());
        }

        ExpeditionRuntime = null;
        IsInitialized = false;
    }
}