using System;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using UnityEngine;

[Serializable]
public sealed class ModifierEffectConfiguration
{
    [SerializeField]
    private ExpeditionModifierTarget target =
        ExpeditionModifierTarget.Enemy;

    [SerializeField]
    private string statId =
        "Damage";

    [SerializeField]
    private float percent =
        1f;

    public ExpeditionModifierTarget Target =>
        target;

    public string StatId =>
        statId;

    public float Percent =>
        percent;
}