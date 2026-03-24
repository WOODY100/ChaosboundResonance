using System.Collections.Generic;
using UnityEngine;

public class PersistentZone : MonoBehaviour
{
    private RuntimeSkill skill;

    private float duration;
    private float tickRate;
    private float timer;
    private float tickTimer;

    private float damage;
    private float radius;

    public System.Action<PersistentZone> OnZoneEnded;

    [SerializeField] private float baseSize = 1f;

    private Collider[] hits = new Collider[100];
    private Dictionary<Collider, IDamageable> cache = new Dictionary<Collider, IDamageable>();
    private Dictionary<IDamageable, float> accumulatedDamage = new Dictionary<IDamageable, float>();

    private float visualTimer;
    private float visualInterval = 0.5f;

    [SerializeField] private LayerMask enemyLayer;

    public void Initialize(RuntimeSkill runtimeSkill)
    {
        skill = runtimeSkill;

        duration = skill.Stats.FinalDuration;
        radius = skill.Stats.FinalImpactRadius;
        tickRate = Mathf.Max(0.1f, skill.Stats.FinalTickRate);

        damage = skill.Stats.FinalDamage;

        // 🔥 ESCALA VISUAL
        transform.localScale = Vector3.one * (radius * 2f / baseSize);

        // 🔥 PARTICLES (ahora sí correcto)
        var ps = GetComponentInChildren<ParticleSystem>();

        if (ps != null)
        {
            var main = ps.main;

            main.duration = duration;
            main.startLifetime = duration;

            ps.Clear();
            ps.Play();
        }
    }

    private void Update()
    {
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
            ApplyAccumulatedDamage();

            OnZoneEnded?.Invoke(this);

            Destroy(gameObject);
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

            if (damageable == null || ((MonoBehaviour)damageable).gameObject.activeInHierarchy == false)
            {
                cache.Remove(col);
                continue;
            }

            if (!accumulatedDamage.ContainsKey(damageable))
            {
                accumulatedDamage[damageable] = 0f;
            }

            accumulatedDamage[damageable] += damage;
        }
    }

    private void ApplyAccumulatedDamage()
    {
        foreach (var pair in accumulatedDamage)
        {
            var target = pair.Key;
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

}