using UnityEngine;

[System.Serializable]
public struct ModifierEntry
{
    public SkillModifierType Type;
    public float Value;
}

[CreateAssetMenu(menuName = "Skills/Skill Modifier")]
public class SkillModifierDefinition : ScriptableObject
{
    // =========================
    // PROPERTIES
    // =========================

    /// <summary>
    /// Returns true when this modifier uses the new multi-modifier format.
    /// </summary>
    public bool UsesMultiModifier =>
        modifiers != null && modifiers.Length > 0;

    // =========================
    // UI
    // =========================

    [Header("UI")]
    public string DisplayName;

    [TextArea]
    public string Description;

    public Sprite Icon;

    public SkillRarity Rarity;

    // =========================
    // LEGACY
    // =========================

    [Header("Single Modifier (Legacy)")]
    public SkillModifierType ModifierType;

    public float Value;

    // =========================
    // MULTI MODIFIER
    // =========================

    [Header("Multi Modifier")]
    [SerializeField]
    private ModifierEntry[] modifiers = new ModifierEntry[0];

    public ModifierEntry[] Modifiers => modifiers;

    // =========================
    // BEHAVIOR
    // =========================

    [Header("Behavior")]
    [Tooltip("Allows this modifier to appear multiple times during a run.")]
    public bool IsStackable = true;

    private void OnValidate()
    {
        // Evita referencias nulas
        modifiers ??= new ModifierEntry[0];

        // Si usa el sistema nuevo, limpiar entradas inválidas.
        for (int i = 0; i < modifiers.Length; i++)
        {
            ModifierEntry entry = modifiers[i];

            // Aquí podremos añadir validaciones específicas por tipo
            // cuando el sistema de modificadores esté completamente cerrado.

            modifiers[i] = entry;
        }
    }
}