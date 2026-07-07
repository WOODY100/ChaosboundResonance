using System.Collections.Generic;
using UnityEngine;

public class PersistentZone : PooledBehaviour
{
    private RuntimeSkill skill;

    private float duration;
    private float tickRate;
    private float timer;
    private float tickTimer;

    private float damage;
    private float radius;

    public System.Action<PersistentZone> OnZoneEnded;

    [Header("Visual")]
    [SerializeField] private float visualHeight = 0.18f;

    [SerializeField] private float baseSize = 1f;
    [SerializeField] private LayerMask enemyLayer;

    private readonly Collider[] hits = new Collider[100];
    private readonly Dictionary<Collider, IDamageable> cache = new();
    private readonly Dictionary<IDamageable, float> accumulatedDamage = new();

    private float visualTimer;
    private readonly float visualInterval = 0.5f;

    private ParticleSystem particle;

    protected override void Awake()
    {
        base.Awake();
        particle = GetComponentInChildren<ParticleSystem>();
    }

    public void Initialize(RuntimeSkill runtimeSkill)
    {
        ResetPooledState();

        skill = runtimeSkill;

        duration = skill.Stats.FinalDuration;
        radius = skill.Stats.FinalImpactRadius;
        tickRate = Mathf.Max(0.1f, skill.Stats.FinalTickRate);
        damage = skill.Stats.FinalDamage;

        transform.localScale = Vector3.one * (radius * 2f / baseSize);

        Vector3 pos = transform.position;
        pos.y += visualHeight;
        transform.position = pos;

        if (particle != null)
        {
            ParticleSystem.MainModule main = particle.main;

            main.duration = duration;
            main.startLifetime = duration;

            particle.Clear();
            particle.Play();
        }
    }

    private void Update()
    {
        if (skill == null)
            return;

        timer += Time.deltaTime;
        tickTimer += Time.deltaTime;

        int iterations = 0;

        while (tickTimer >= tickRate && iterations < 5)
        {
            ApplyDamage();
            tickTimer -= tickRate;
            iterations++;
        }

        visualTimer += Time.deltaTime;

        if (visualTimer >= visualInterval)
        {
            ApplyAccumulatedDamage();
            visualTimer = 0f;
        }

        if (timer >= duration)
        {
            EndZone();
        }
    }

    private void ApplyDamage()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            radius,
            hits,
            enemyLayer
        );

        for (int i = 0; i < count; i++)
        {
            Collider col = hits[i];

            if (!cache.TryGetValue(col, out IDamageable damageable))
            {
                damageable = col.GetComponentInParent<IDamageable>();

                if (damageable == null)
                    continue;

                cache[col] = damageable;
            }

            if (damageable == null ||
                ((MonoBehaviour)damageable).gameObject.activeInHierarchy == false)
            {
                cache.Remove(col);
                continue;
            }

            if (!accumulatedDamage.ContainsKey(damageable))
                accumulatedDamage[damageable] = 0f;

            accumulatedDamage[damageable] += damage;
        }
    }

    private void ApplyAccumulatedDamage()
    {
        foreach (var pair in accumulatedDamage)
        {
            IDamageable target = pair.Key;
            float totalDamage = pair.Value;

            if (target == null)
                continue;

            target.TakeDamage(new DamageData(
                totalDamage,
                skill.Definition.DamageType,
                false
            ));
        }

        accumulatedDamage.Clear();
    }

    public void ForceEnd()
    {
        EndZone();
    }

    private void EndZone()
    {
        ApplyAccumulatedDamage();

        OnZoneEnded?.Invoke(this);
        OnZoneEnded = null;

        ReturnToPool();
    }

    protected override void ResetPooledState()
    {
        skill = null;

        duration = 0f;
        tickRate = 0f;
        timer = 0f;
        tickTimer = 0f;
        damage = 0f;
        radius = 0f;
        visualTimer = 0f;

        cache.Clear();
        accumulatedDamage.Clear();

        OnZoneEnded = null;
    }
}