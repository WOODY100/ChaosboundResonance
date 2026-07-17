using System.Collections;
using UnityEngine;

public class StormfallStrike : PooledBehaviour, IAreaStrike
{
    [Header("Visual")]
    [SerializeField] private GameObject lightningImpact;

    [Header("Timing")]
    [SerializeField] private float beamLifetime = 0.18f;
    [SerializeField] private float totalLifetime = 1.2f;

    private RuntimeSkill skill;
    private float damage;
    private DamageType damageType;

    private Coroutine activeRoutine;
    private ParticleSystem[] particleSystems;

    private static readonly Collider[] hitBuffer = new Collider[64];

    protected override void Awake()
    {
        base.Awake();

        particleSystems = GetComponentsInChildren<ParticleSystem>(true);

        if (lightningImpact == null)
            lightningImpact = transform.Find("LightningImpact")?.gameObject;
    }

    protected override void ResetPooledState()
    {
        damageType = DamageType.Physical;

        if (lightningImpact != null)
            lightningImpact.SetActive(true);

        if (particleSystems != null)
        {
            foreach (ParticleSystem ps in particleSystems)
            {
                if (ps == null)
                    continue;

                ps.Clear(true);
                ps.Play(true);
            }
        }

        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;

        skill = null;
        damage = 0f;
        activeRoutine = null;
    }

    public void Initialize(RuntimeSkill runtimeSkill)
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        ResetPooledState();

        if (runtimeSkill == null)
        {
            Debug.LogError("StormfallStrike initialized with null RuntimeSkill.");
            ReturnToPool();
            return;
        }

        skill = runtimeSkill;

        damage = Mathf.Max(0f, skill.Stats.FinalDamage);
        damageType = skill.Definition.DamageType;

        activeRoutine = StartCoroutine(StrikeRoutine());
    }

    private IEnumerator StrikeRoutine()
    {
        ApplyDamage();

        yield return new WaitForSeconds(beamLifetime);

        if (lightningImpact != null)
            lightningImpact.SetActive(false);

        float remainingLifetime = Mathf.Max(
            0f,
            totalLifetime - beamLifetime);

        yield return new WaitForSeconds(remainingLifetime);

        ReturnToPool();
    }

    private void ApplyDamage()
    {
        if (skill == null)
            return;

        float radius = Mathf.Max(
            0.1f,
            skill.Stats.FinalImpactRadius);

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            radius,
            hitBuffer,
            GameLayers.Enemy
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = hitBuffer[i];

            if (hit == null)
                continue;

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable == null || damageable.IsDead)
                continue;

            damageable.TakeDamage(new DamageData(
                damage,
                damageType
            ));
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        ResetPooledState();
    }
}