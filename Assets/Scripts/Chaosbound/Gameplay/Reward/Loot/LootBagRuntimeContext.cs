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

        IsInitialized = true;
    }

    private void OnDisable()
    {
        ExpeditionRuntime = null;
        IsInitialized = false;
    }
}