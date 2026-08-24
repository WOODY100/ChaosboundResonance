using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Shared.Contracts;
using Chaosbound.Shared.Enums;
using Chaosbound.Shared.Identifiers;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chaosbound/Enemies/Enemy Variant")]
public class EnemyVariantData :
    ScriptableObject,
    IMaterializableReference,
    ISpawnPrefabReference
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
    private EnemyCombatType m_CombatType =
        EnemyCombatType.Melee;

    [SerializeField]
    private EnemyRole[] m_Roles;

    [SerializeField]
    private EnemyTier m_Tier = EnemyTier.Tier1;

    public EnemyCategory Category =>
        m_Category;

    public EnemyCombatType CombatType =>
        m_CombatType;

    public IReadOnlyList<EnemyRole> Roles =>
        m_Roles;

    public EnemyTier Tier =>
        m_Tier;

    [Header("Movement")]

    [SerializeField]
    private EnemyMovementPolicyType m_MovementPolicy =
        EnemyMovementPolicyType.Approach;

    [SerializeField]
    private float m_PreferredDistance = 2f;

    [SerializeField]
    private float m_DistanceTolerance = 0.5f;

    public EnemyMovementPolicyType MovementPolicy =>
        m_MovementPolicy;

    public float PreferredDistance =>
        m_PreferredDistance;

    public float DistanceTolerance =>
        m_DistanceTolerance;

    [Header("Combat")]

    [SerializeField]
    private EnemyAttackDefinition m_Attack;

    public EnemyAttackDefinition Attack =>
        m_Attack;

    [Header("Rewards")]

    [SerializeField]
    private EnemyRewardDefinition m_Rewards;

    public EnemyRewardDefinition Rewards =>
        m_Rewards;

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