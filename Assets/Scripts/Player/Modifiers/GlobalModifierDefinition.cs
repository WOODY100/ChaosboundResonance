using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Global Modifier")]
public class GlobalModifierDefinition : ScriptableObject
{
    public StatType TargetStat;
    public ModifierType ModifierType;
    public float Value;
}