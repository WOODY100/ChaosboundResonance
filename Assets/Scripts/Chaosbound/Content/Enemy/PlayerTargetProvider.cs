using System;
using UnityEngine;

public sealed class PlayerTargetProvider :
    ITargetProvider
{
    private readonly Transform player;

    public PlayerTargetProvider(
        Transform player)
    {
        this.player =
            player
            ?? throw new ArgumentNullException(
                nameof(player));
    }

    public Transform GetTarget()
    {
        return player;
    }
}