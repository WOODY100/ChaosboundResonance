using Chaosbound.Gameplay.Threat.ValueObjects;
using Chaosbound.Shared.Identifiers;
using UnityEngine;

[CreateAssetMenu(menuName = "Arena/Enemy Variant")]
public class EnemyVariantData : ScriptableObject
{
    [Header("Identity")]

    [SerializeField]
    private string m_ContentId;

    [SerializeField]
    private string m_DisplayName;

    public ContentId Id => new(m_ContentId);

    public string DisplayName => m_DisplayName;

    [Header("Stats")]
    public float baseHealth = 10f;
    public float baseDamage = 5f;
    public float moveSpeed = 3.5f;

    [Header("Classification")]
    public EnemyCategory category;

    public EnemyRole[] roles;

    public TacticalCapability[] tacticalCapabilities;

    [Header("Threat")]

    [SerializeField]
    private float m_ThreatCost = 10f;

    public ThreatCost ThreatCost => new(m_ThreatCost);

    [Header("Rewards")]
    public int experienceReward = 5;
    public int goldReward = 1;

    [Header("Visual")]
    public GameObject modelPrefab;
}