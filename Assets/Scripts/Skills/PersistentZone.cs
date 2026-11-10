using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using Chaosbound.Gameplay.Debugging.SkillGizmos;
#endif

public class PersistentZone : PooledBehaviour
{
    private RuntimeSkill skill;
    private string executionId;

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

    [Header("Debug - Damage Area")]
    [SerializeField] private bool showDamageRadiusGizmo = true;
    [SerializeField, Min(12)] private int gizmoSegments = 64;

    private readonly Collider[] hits = new Collider[100];
    private readonly Dictionary<Collider, IDamageable> cache = new();
    private readonly Dictionary<IDamageable, float> accumulatedDamage = new();

    private readonly HashSet<IDamageable> damagedThisTick = new();

    private float visualTimer;
    private readonly float visualInterval = 0.5f;

    private ParticleSystem particle;

    protected override void Awake()
    {
        base.Awake();
        particle = GetComponentInChildren<ParticleSystem>();
    }

    public void Initialize(RuntimeSkill runtimeSkill, string castExecutionId)
    {
        ResetPooledState();

        if (runtimeSkill == null)
        {
            Debug.LogError("PersistentZone initialized with null RuntimeSkill.");
            ReturnToPool();
            return;
        }

        skill = runtimeSkill;
        executionId = castExecutionId;
        radius = Mathf.Max(0.1f, skill.Stats.FinalImpactRadius);
        tickRate = Mathf.Max(0.1f, skill.Stats.FinalTickRate);
        duration = Mathf.Max(0.01f, skill.Stats.FinalDuration);
        damage = Mathf.Max(0f, skill.Stats.FinalDamage);

        float safeBaseSize = Mathf.Max(0.01f, baseSize);
        transform.localScale = Vector3.one * (radius * 2f / safeBaseSize);

        Vector3 pos = transform.position;
        pos.y = Mathf.Max(pos.y, visualHeight);
        transform.position = pos;

        if (particle != null)
        {
            particle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

            ParticleSystem.MainModule main = particle.main;

            main.duration = duration;
            main.startLifetime = duration;

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

        damagedThisTick.Clear();

        for (int i = 0; i < count; i++)
        {
            Collider col = hits[i];

            if (col == null)
                continue;

            if (!cache.TryGetValue(col, out IDamageable damageable))
            {
                damageable = col.GetComponentInParent<IDamageable>();

                if (damageable == null)
                    continue;

                cache[col] = damageable;
            }

            MonoBehaviour targetBehaviour = damageable as MonoBehaviour;

            if (targetBehaviour == null ||
                !targetBehaviour.gameObject.activeInHierarchy)
            {
                cache.Remove(col);
                continue;
            }

            // Un mismo objetivo solamente recibe una contribución por tick.
            if (!damagedThisTick.Add(damageable))
                continue;

            if (!accumulatedDamage.ContainsKey(damageable))
                accumulatedDamage[damageable] = 0f;

            accumulatedDamage[damageable] += damage;
        }
    }

    private void ApplyAccumulatedDamage()
    {
        if (skill == null)
            return;

        foreach (var pair in accumulatedDamage)
        {
            IDamageable target = pair.Key;
            float totalDamage = pair.Value;

            if (target == null)
                continue;

            string skillId = null;
            string readableSkillId = null;
            string skillDisplayName = null;
#if UNITY_EDITOR
            skillId = SkillDamageAttribution.GetSkillDefinitionId(skill.Definition);
            readableSkillId = SkillDamageAttribution.GetReadableSkillId(skill.Definition);
            skillDisplayName = SkillDamageAttribution.GetSkillDisplayName(skill.Definition);
#endif
            target.TakeDamage(new DamageData(
                totalDamage, false, skillId, executionId,
                null, readableSkillId, skillDisplayName));
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
        executionId = null;

        duration = 0f;
        tickRate = 0f;
        timer = 0f;
        tickTimer = 0f;
        damage = 0f;
        radius = 0f;
        visualTimer = 0f;

        cache.Clear();
        accumulatedDamage.Clear();
        damagedThisTick.Clear();

        transform.localScale = Vector3.one;

        Vector3 pos = transform.position;
        pos.y = 0f;
        transform.position = pos;

        OnZoneEnded = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showDamageRadiusGizmo || radius <= 0f ||
            !SkillGizmoUtility.ShouldDraw(this, false, SkillGizmoUtility.Category.Player))
            return;

        Vector3 center = transform.position;
        Color damageColor = new Color(1f, 0.78f, 0.15f, 0.9f);
        Color footprintColor = new Color(0.1f, 0.9f, 1f, 1f);

        SkillGizmoUtility.DrawWireSphere(center, radius, damageColor);
        SkillGizmoUtility.DrawGroundCircle(center, radius, footprintColor, gizmoSegments);
        if (SkillGizmoUtility.IsLayerEnabled(SkillGizmoUtility.VisualLayer.Geometry))
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(center - Vector3.right * 0.15f, center + Vector3.right * 0.15f);
            Gizmos.DrawLine(center - Vector3.forward * 0.15f, center + Vector3.forward * 0.15f);
        }
    }
#endif
}
