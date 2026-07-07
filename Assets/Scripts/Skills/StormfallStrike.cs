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
    private const int EnemyLayerMask = 1 << 6;

    protected override void Awake()
    {
        base.Awake();

        particleSystems = GetComponentsInChildren<ParticleSystem>(true);

        if (lightningImpact == null)
            lightningImpact = transform.Find("LightningImpact")?.gameObject;
    }

    protected override void ResetPooledState()
    {
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

        skill = null;
        damage = 0f;
        activeRoutine = null;
    }

    public void Initialize(RuntimeSkill runtimeSkill)
    {
        skill = runtimeSkill;
        damage = skill.Stats.FinalDamage;
        damageType = skill.Definition.DamageType;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(StrikeRoutine());
    }

    private IEnumerator StrikeRoutine()
    {
        ApplyDamage();

        yield return new WaitForSeconds(beamLifetime);

        if (lightningImpact != null)
            lightningImpact.SetActive(false);

        yield return new WaitForSeconds(totalLifetime - beamLifetime);

        ReturnToPool();
    }

    private void ApplyDamage()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            skill.Stats.FinalImpactRadius,
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

        if (lightningImpact != null)
            lightningImpact.SetActive(true);
    }
}