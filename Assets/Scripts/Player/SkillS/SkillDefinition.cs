using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Definition")]
public class SkillDefinition : ScriptableObject
{
    public bool HasModifiers => possibleModifiers is { Count: > 0 };

    public bool HasEvolutions => evolutions is { Count: > 0 };

    // =========================
    // UI
    // =========================
    [Header("UI")]
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private SkillRarity rarity;

    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public SkillRarity Rarity => rarity;

    // =========================
    // EXECUTION
    // =========================
    [Header("Execution Settings")]
    [Tooltip("Visual object instantiated by the executor.")]
    public GameObject ExecutionPrefab;   // El VFX / lógica visual

    [Tooltip("Executor responsible for the skill logic.")]
    public GameObject ExecutorPrefab;    // El executor lógico
    public DamageType DamageType;

    // =========================
    // BASE STATS
    // =========================
    [Header("Base Stats")]
    [Tooltip("Base damage before upgrades.")]
    public float BaseDamage = 1f;
    [Tooltip("Time between casts.")]
    public float BaseCooldown = 1f;
    public float BaseSpawnRadius = 0f;
    public float BaseImpactRadius = 0f;
    [Tooltip("Maximum targeting distance.")]
    public float BaseRange;
    [Tooltip("Duration of persistent skills.")]
    public float BaseDuration;
    public float BaseTickRate = 1f;
    public int BaseCount = 1;

    // =========================
    // BEHAVIOR FLAGS
    // =========================
    [Header("Behavior Settings")]
    public bool ScalesWithAttackSpeed = true;
    public bool CanCrit = false;
    public bool IsDamageOverTime = false;
    
    [Header("Cooldown Behavior")]
    [Tooltip("Cooldown begins after the skill finishes instead of immediately.")]
    public bool CooldownStartsAfterDuration;

    // =========================
    // MODIFIERS
    // =========================
    [Header("Modifiers Pool")]
    [SerializeField]
    private List<SkillModifierDefinition> possibleModifiers = new();

    public IReadOnlyList<SkillModifierDefinition> PossibleModifiers => possibleModifiers;

    // =========================
    // EVOLUTIONS
    // =========================
    [Header("Evolutions")]
    [SerializeField]
    private List<SkillEvolutionDefinition> evolutions = new();

    public IReadOnlyList<SkillEvolutionDefinition> Evolutions => evolutions;

    private void OnValidate()
    {
        possibleModifiers ??= new List<SkillModifierDefinition>();
        evolutions ??= new List<SkillEvolutionDefinition>();

        BaseDamage = Mathf.Max(0f, BaseDamage);
        BaseCooldown = Mathf.Max(0.05f, BaseCooldown);

        BaseSpawnRadius = Mathf.Max(0f, BaseSpawnRadius);
        BaseImpactRadius = Mathf.Max(0f, BaseImpactRadius);
        BaseRange = Mathf.Max(0f, BaseRange);
        BaseDuration = Mathf.Max(0f, BaseDuration);

        BaseCount = Mathf.Max(1, BaseCount);

        if (ExecutionPrefab == null)
            Debug.LogWarning($"{name}: ExecutionPrefab is missing.", this);

        if (ExecutorPrefab == null)
            Debug.LogWarning($"{name}: ExecutorPrefab is missing.", this);
    }
}