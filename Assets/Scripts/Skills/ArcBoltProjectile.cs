using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArcBoltProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private GameObject impactVFX;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 18f;

    [Header("Lifetime")]
    [SerializeField] private float maxLifetime = 5f;

    private RuntimeSkill skill;
    private PlayerStats playerStats;
    private Rigidbody rb;

    private int remainingPenetration;

    private bool initialized = false;
    private bool hasImpacted = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // =====================================================
    // INITIALIZATION
    // =====================================================
    public void Initialize(
        RuntimeSkill runtimeSkill,
        Vector3 direction,
        PlayerStats ownerStats)
    {
        skill = runtimeSkill;
        playerStats = ownerStats;

        float attackSpeedMultiplier = 1f;

        if (skill.Definition.ScalesWithAttackSpeed && playerStats != null)
        {
            attackSpeedMultiplier = playerStats.FinalAttackSpeed;
        }

        float finalSpeed = baseSpeed * attackSpeedMultiplier;

        rb.linearVelocity = direction * finalSpeed;

        remainingPenetration = skill.Stats.PenetrationCount;

        initialized = true;

        Destroy(gameObject, maxLifetime);
    }

    // =====================================================
    // COLLISION
    // =====================================================
    private void OnTriggerEnter(Collider other)
    {
        if (!initialized || hasImpacted)
            return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null || damageable.IsDead)
            return;

        hasImpacted = true;

        Vector3 hitPoint = other.ClosestPoint(transform.position);

        ApplyDamage(damageable);

        if (remainingPenetration > 0)
        {
            remainingPenetration--;
            hasImpacted = false;
            return;
        }

        if (impactVFX != null)
            Instantiate(impactVFX, hitPoint, Quaternion.identity);

        Destroy(gameObject);
    }

    // =====================================================
    // DAMAGE LOGIC
    // =====================================================
    private void ApplyDamage(IDamageable target)
    {
        float damage = skill.Stats.FinalDamage;

        // Critical
        if (skill.Stats.CriticalChance > 0f &&
            Random.value < skill.Stats.CriticalChance)
        {
            float critMultiplier =
                skill.Stats.CriticalMultiplier > 0f
                ? skill.Stats.CriticalMultiplier
                : 2f;

            damage *= critMultiplier;
        }

        DamageData damageData = new DamageData(
            damage,
            skill.Definition.DamageType
        );

        target.TakeDamage(damageData);
    }
}