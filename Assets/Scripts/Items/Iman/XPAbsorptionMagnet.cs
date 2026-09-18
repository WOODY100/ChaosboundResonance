using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System.Collections.Generic;
using UnityEngine;

public sealed class XPAbsorptionMagnet : AutoPickupBehaviour
{
    public const string ContentId =
        "xp_absorption_magnet";

    protected override float GetPickupRadius()
    {
        return GetPlayerPickupRadius();
    }

    protected override void OnAutoPickupTriggered()
    {
        if (Player == null)
            return;

        if (RunManager.Instance == null)
            return;

        ExpeditionRuntimeState runtimeState =
            RunManager.Instance.ExpeditionRuntimeState;

        if (runtimeState == null)
            return;

        IReadOnlyList<ResonanceFragmentPickup> fragments =
            runtimeState.XPFragments.CaptureActiveFragments();

        for (int i = 0; i < fragments.Count; i++)
        {
            ResonanceFragmentPickup fragment =
                fragments[i];

            if (fragment == null)
                continue;

            if (!fragment.gameObject.activeInHierarchy)
                continue;

            fragment.Attract();
        }

        ReturnToPool();
    }
}