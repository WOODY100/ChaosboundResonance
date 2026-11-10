using UnityEngine;
#if UNITY_EDITOR
using Chaosbound.Gameplay.Debugging.SkillGizmos;
#endif

[RequireComponent(typeof(PooledObject))]
public sealed class BossAreaEffectVisual : MonoBehaviour
{
    private float remainingDuration;
    private bool initialized;
    private float damageRadius;

    private void OnEnable()
    {
        remainingDuration = 0f;
        initialized = false;
        damageRadius = 0f;
    }

    public bool Initialize(float duration, float radius)
    {
        if (duration <= 0f || radius <= 0f)
        {
            ReturnToPool();
            return false;
        }

        remainingDuration = duration;
        damageRadius = radius;
        initialized = true;
        return true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        remainingDuration -= Time.deltaTime;
        if (remainingDuration <= 0f)
            ReturnToPool();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!SkillGizmoUtility.ShouldDraw(this, false, SkillGizmoUtility.Category.Boss) || damageRadius <= 0f)
            return;

        // This is the exact OverlapSphere radius used by BossAreaAttackExecutor.
        SkillGizmoUtility.DrawWireSphere(
            transform.position,
            damageRadius,
            new Color(1f, 0.25f, 0.1f, 0.95f));
        SkillGizmoUtility.DrawGroundCircle(
            transform.position,
            damageRadius,
            new Color(1f, 0.25f, 0.1f, 0.8f));
    }
#endif

    private void ReturnToPool()
    {
        PooledObject pooled = GetComponent<PooledObject>();
        if (pooled != null)
            pooled.ReturnToPool();
        else
            Destroy(gameObject);
    }
}
