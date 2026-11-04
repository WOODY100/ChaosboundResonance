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
    private string executionId;

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
        executionId = null;
        activeRoutine = null;
    }

    public void Initialize(RuntimeSkill runtimeSkill, string castExecutionId)
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
        executionId = castExecutionId;

        damage = Mathf.Max(0f, skill.Stats.FinalDamage);

        activeRoutine = StartCoroutine(StrikeRoutine());
    }

    private IEnumerator StrikeRoutine()
    {
        if (skill != null &&
            SkillAudioPlayer.Instance != null)
        {
            SkillAudioPlayer.Instance.PlaySkillEvent(
                skill.Definition.AudioDefinition);
        }

        ApplyDamage();

        yield return new WaitForSeconds(beamLifetime);

        if (lightningImpact != null)
            lightningImpact.SetActive(false);

        float remainingLifetime =
            Mathf.Max(0f, totalLifetime - beamLifetime);

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

            string skillId = null;
            string readableSkillId = null;
            string skillDisplayName = null;
#if UNITY_EDITOR
            skillId = SkillDamageAttribution.GetSkillDefinitionId(skill.Definition);
            readableSkillId = SkillDamageAttribution.GetReadableSkillId(skill.Definition);
            skillDisplayName = SkillDamageAttribution.GetSkillDisplayName(skill.Definition);
#endif
            damageable.TakeDamage(new DamageData(
                damage, false, skillId, executionId,
            null, readableSkillId, skillDisplayName));
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