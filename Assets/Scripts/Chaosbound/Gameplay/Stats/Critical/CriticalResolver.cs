using UnityEngine;

public static class CriticalResolver
{
    public static CriticalResult Resolve(
        bool canCrit,
        float playerCritChance,
        float skillCritChance,
        float playerCritDamage,
        float skillCritDamageBonus)
    {
        if (!canCrit)
            return CriticalResult.NoCritical;

        float effectiveChance =
            Mathf.Clamp01(
                playerCritChance +
                skillCritChance);

        if (effectiveChance <= 0f)
            return CriticalResult.NoCritical;

        bool isCritical =
            Random.value < effectiveChance;

        if (!isCritical)
            return CriticalResult.NoCritical;

        float effectiveDamageMultiplier =
            Mathf.Max(
                1f,
                playerCritDamage +
                skillCritDamageBonus);

        return new CriticalResult(
            true,
            effectiveDamageMultiplier);
    }
}