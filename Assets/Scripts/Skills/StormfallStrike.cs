using System.Collections;
using UnityEngine;

public class StormfallStrike : MonoBehaviour, IAreaStrike
{
    [SerializeField] private GameObject lightningImpactPrefab;
    [SerializeField] private float beamLifetime = 0.18f;

    private RuntimeSkill skill;
    private float damage;
    private DamageType damageType;


    public void Initialize(RuntimeSkill runtimeSkill)
    {
        skill = runtimeSkill;
        damage = skill.Stats.FinalDamage;
        damageType = skill.Definition.DamageType;

        StartCoroutine(StrikeRoutine());
    }

    private IEnumerator StrikeRoutine()
    {
        ApplyDamage();

        Transform beam = transform.Find("LightningImpact");

        if (beam != null)
            Destroy(beam.gameObject, beamLifetime);

        // Esperar a que GroundImpact termine
        yield return new WaitForSeconds(1.2f);

        Destroy(gameObject);
    }

    private void ApplyDamage()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            skill.Stats.FinalImpactRadius,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            IDamageable damageable =
                hit.GetComponentInParent<IDamageable>();

            if (damageable == null || damageable.IsDead)
                continue;

            DamageData damageData =
                new DamageData(damage, damageType);

            damageable.TakeDamage(damageData);
        }
    }
}