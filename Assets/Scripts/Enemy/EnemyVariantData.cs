using Chaosbound.Core.Runtime.Enemies;
using UnityEngine;

[CreateAssetMenu(menuName = "Arena/Enemy Variant")]
public class EnemyVariantData : ScriptableObject
{
    [Header("Identity")]
    public string displayName;

    [Header("Stats")]
    public float baseHealth = 10f;
    public float baseDamage = 5f;
    public float moveSpeed = 3.5f;

    [Header("Classification")]
    public EnemyCategory category;

    public EnemyRole[] roles;

    public TacticalCapability[] tacticalCapabilities;

    [Header("Rewards")]
    public int experienceReward = 5;
    public int goldReward = 1;

    [Header("Visual")]
    public GameObject modelPrefab;
}