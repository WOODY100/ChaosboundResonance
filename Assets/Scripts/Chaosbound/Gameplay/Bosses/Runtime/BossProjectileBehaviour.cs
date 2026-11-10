using UnityEngine;
#if UNITY_EDITOR
using Chaosbound.Gameplay.Debugging.SkillGizmos;
#endif

[RequireComponent(typeof(PooledObject))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public sealed class BossProjectileBehaviour : MonoBehaviour
{
    private Rigidbody body;
    private Transform owner;
    private Transform target;
    private float damage;
    private float remainingLifetime;
    private float maxTravelDistance;
    private bool initialized;
    private bool impacted;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        GetComponent<Collider>().isTrigger = true;
        body.useGravity = false;
    }

    private void OnEnable()
    {
        initialized = false;
        impacted = false;
        owner = null;
        target = null;
        damage = 0f;
        remainingLifetime = 0f;
        maxTravelDistance = 0f;

        if (body != null)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    public bool Initialize(
        Transform projectileOwner,
        Transform projectileTarget,
        Vector3 direction,
        float speed,
        float lifetime,
        float damageAmount)
    {
        if (body == null)
            body = GetComponent<Rigidbody>();

        if (projectileOwner == null || projectileTarget == null ||
            direction.sqrMagnitude < 0.0001f ||
            speed <= 0f || lifetime <= 0f || damageAmount <= 0f ||
            body == null || GetComponent<Collider>() == null)
        {
            ReturnToPool();
            return false;
        }

        owner = projectileOwner;
        target = projectileTarget;
        damage = damageAmount;
        remainingLifetime = lifetime;
        maxTravelDistance = speed * lifetime;

        Vector3 normalizedDirection = direction.normalized;
        transform.rotation = Quaternion.LookRotation(normalizedDirection);
        body.linearVelocity = normalizedDirection * speed;
        initialized = true;
        return true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        remainingLifetime -= Time.deltaTime;
        if (remainingLifetime <= 0f)
            ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!initialized || impacted || other == null)
            return;

        Transform hitTransform = other.transform;

        if ((owner != null && (hitTransform == owner || hitTransform.IsChildOf(owner))) ||
            target == null ||
            !(hitTransform == target || hitTransform.IsChildOf(target) || target.IsChildOf(hitTransform)))
            return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null || damageable.IsDead)
            return;

        impacted = true;

        DamageData damageData = new DamageData(damage);
        damageData.source = owner != null ? owner.gameObject : null;
        damageData.analyticsSourceCategory = "BossProjectile";
        damageable.TakeDamage(damageData);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        PooledObject pooled = GetComponent<PooledObject>();
        if (pooled != null)
            pooled.ReturnToPool();
        else
            Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!SkillGizmoUtility.ShouldDraw(this, false, SkillGizmoUtility.Category.Boss))
            return;

        SkillGizmoUtility.DrawCollider(
            GetComponent<Collider>(), new Color(1f, 0.75f, 0.15f, 0.95f));
        if (initialized)
            SkillGizmoUtility.DrawDirection(
                transform.position, transform.forward,
                maxTravelDistance > 0f ? maxTravelDistance : 1.5f,
                Color.cyan);
    }
#endif
}
