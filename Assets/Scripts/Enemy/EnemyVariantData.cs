using Chaosbound.Gameplay.EnemySolver.Enums;
using Chaosbound.Gameplay.Threat.ValueObjects;
using Chaosbound.Shared.Contracts;
using Chaosbound.Shared.Identifiers;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arena/Enemy Variant")]
public class EnemyVariantData : ScriptableObject, IMaterializableReference
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

    [SerializeField]
    private EnemyCategory m_Category;

    [SerializeField]
    private EnemyRole[] m_Roles;

    [SerializeField]
    private TacticalCapability[] m_TacticalCapabilities;

    public EnemyCategory Category => m_Category;

    public IReadOnlyList<EnemyRole> Roles => m_Roles;

    public IReadOnlyList<TacticalCapability> TacticalCapabilities =>
        m_TacticalCapabilities;

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