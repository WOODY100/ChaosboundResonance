using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Definition")]
public class SkillDefinition : ScriptableObject
{
    // =========================
    // UI
    // =========================
    [Header("UI")]
    public string DisplayName;
    [TextArea] public string Description;
    public Sprite Icon;
    public SkillRarity Rarity;

    // =========================
    // EXECUTION
    // =========================
    [Header("Execution")]
    public GameObject ExecutionPrefab;   // El VFX / lógica visual
    public GameObject ExecutorPrefab;    // El executor lógico
    public DamageType DamageType;

    // =========================
    // BASE STATS
    // =========================
    [Header("Base Stats")]
    public float BaseDamage = 1f;
    public float BaseCooldown = 1f;
    public float BaseSpawnRadius = 0f;
    public float BaseImpactRadius = 0f;
    public float BaseRange = 0f;
    public float BaseDuration = 0f;
    public int BaseCount = 1;

    // =========================
    // BEHAVIOR FLAGS
    // =========================
    [Header("Behavior")]
    public bool ScalesWithAttackSpeed = true;
    public bool CanCrit = false;
    public bool IsDamageOverTime = false;
    
    [Header("Cooldown Behavior")]
    public bool CooldownStartsAfterDuration = false;

    // =========================
    // MODIFIERS
    // =========================
    [Header("Modifiers Pool")]
    public List<SkillModifierDefinition> PossibleModifiers;

    // =========================
    // EVOLUTIONS
    // =========================
    [Header("Evolutions")]
    public List<SkillEvolutionDefinition> Evolutions;
}