using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Arena/Arena Definition")]
public class ArenaDefinition : ScriptableObject
{
    [Header("Identity")]
    public string arenaName;
    public string sceneName;

    [Header("Unlock Requirements")]
    public ArenaDefinition previousArena;
    public int requiredPlayerLevel;
    public int requiredEquipmentTier;

    [Header("Run Settings")]
    public float duration = 1500f; // 25 minutos
    public float finalBossTime = 1450f;

    [Header("Scaling Curves")]
    public AnimationCurve spawnRate;
    public AnimationCurve maxEnemies;
    public AnimationCurve speedMultiplier;

    [Header("Pressure Settings")]
    public int maxAttackSlots = 6;
    public float spawnRadiusMin = 12f;
    public float spawnRadiusMax = 18f;
}