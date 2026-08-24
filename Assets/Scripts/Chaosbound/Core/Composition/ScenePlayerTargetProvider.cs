using Chaosbound.Core.Composition;
using System;
using UnityEngine;

public sealed class ScenePlayerTargetProvider :
    ITargetProvider
{
    public Transform GetTarget()
    {
        ExpeditionSceneContext scene =
            ExpeditionSceneContext.Current;

        if (scene == null)
        {
            throw new InvalidOperationException(
                "ExpeditionSceneContext is not available.");
        }

        if (scene.Player == null)
        {
            throw new InvalidOperationException(
                "ExpeditionSceneContext.Player is not available.");
        }

        return scene.Player.transform;
    }
}