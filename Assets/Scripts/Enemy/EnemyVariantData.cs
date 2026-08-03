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

    [SerializeField]
    private float m_BaseHealth = 10f;

    [SerializeField]
    private float m_BaseDamage = 5f;

    [SerializeField]
    private float m_MoveSpeed = 3.5f;

    public float BaseHealth => m_BaseHealth;

    public float BaseDamage => m_BaseDamage;

    public float MoveSpeed => m_MoveSpeed;

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

    [SerializeField]
    private int m_ExperienceReward = 5;

    [SerializeField]
    private int m_GoldReward = 1;

    public int ExperienceReward => m_ExperienceReward;

    public int GoldReward => m_GoldReward;

    [Header("Visual")]

    /// <summary>
    /// Visual model associated with this enemy variant.
    /// Used by UI, previews and other visual systems.
    /// </summary>
    [SerializeField]
    private GameObject m_ModelPrefab;

    public GameObject ModelPrefab => m_ModelPrefab;

    [Header("Spawn")]

    /// <summary>
    /// Gameplay prefab that should be materialized by
    /// the Spawn Runtime.
    /// </summary>
    [SerializeField]
    private GameObject m_SpawnPrefab;

    public GameObject SpawnPrefab => m_SpawnPrefab;
}