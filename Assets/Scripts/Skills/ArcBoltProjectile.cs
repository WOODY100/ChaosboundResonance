using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArcBoltProjectile : PooledBehaviour, IProjectile
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
    private bool initialized;
    private bool hasImpacted;
    private float lifetimeTimer;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    protected override void ResetPooledState()
    {
        initialized = false;
        hasImpacted = false;
        lifetimeTimer = 0f;
        remainingPenetration = 0;
        skill = null;
        playerStats = null;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (!initialized)
            return;

        lifetimeTimer += Time.deltaTime;

        if (lifetimeTimer >= maxLifetime)
            ReturnToPool();
    }

    public void Initialize(
        RuntimeSkill runtimeSkill,
        Vector3 direction,
        PlayerStats ownerStats)
    {
        ResetPooledState();

        skill = runtimeSkill;
        playerStats = ownerStats;

        float attackSpeedMultiplier = 1f;

        if (skill.Definition.ScalesWithAttackSpeed && playerStats != null)
            attackSpeedMultiplier = playerStats.FinalAttackSpeed;

        float finalSpeed = baseSpeed * attackSpeedMultiplier;

        rb.linearVelocity = direction * finalSpeed;
        remainingPenetration = skill.Stats.PenetrationCount;

        initialized = true;
    }

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

        SpawnImpactVFX(hitPoint);
        ReturnToPool();
    }

    private void SpawnImpactVFX(Vector3 hitPoint)
    {
        if (impactVFX == null)
            return;

        PoolManager.Instance.Get(
            impactVFX,
            hitPoint,
            Quaternion.identity
        );
    }

    private void ApplyDamage(IDamageable target)
    {
        float damage = skill.Stats.FinalDamage;

        if (skill.Stats.CriticalChance > 0f &&
            Random.value < skill.Stats.CriticalChance)
        {
            float critMultiplier =
                skill.Stats.CriticalMultiplier > 0f
                    ? skill.Stats.CriticalMultiplier
                    : 2f;

            damage *= critMultiplier;
        }

        target.TakeDamage(new DamageData(
            damage,
            skill.Definition.DamageType
        ));
    }
}