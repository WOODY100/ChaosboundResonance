using UnityEngine;

[System.Serializable]
public class SkillStats
{
    // =========================
    // BASE VALUES
    // =========================
    public float BaseDamage;
    public float BaseCooldown;
    public float BaseSpawnRadius;
    public float BaseRange;
    public float BaseDuration;
    public float BaseTickRate;
    public int BaseCount;

    // =========================
    // DAMAGE LAYERS
    // =========================
    public float FlatDamage;
    public float PercentDamage;
    public float FinalDamageMultiplier = 1f;

    public float CriticalChance;
    public float CriticalMultiplier;

    // =========================
    // COOLDOWN
    // =========================
    public float FlatCooldownReduction;
    public float PercentCooldownReduction;

    // =========================
    // AREA / COVERAGE
    // =========================
    public float PercentSpawnRadius;
    public float PercentRange;
    public float PercentDuration;
    public float PercentTickRate;

    public int ExtraCount;
    public int PenetrationCount;
    public int BounceCount;
    public int ChainCount;

    // =========================
    // FLAGS
    // =========================
    public bool GrantsExplosion;
    public bool GrantsChaining;
    public bool GrantsSplit;

    // =========================
    // FINAL CACHED VALUES
    // =========================
    public float FinalDamage { get; private set; }
    public float FinalCooldown { get; private set; }
    public float FinalSpawnRadius { get; private set; }
    public float FinalRange { get; private set; }
    public float FinalDuration { get; private set; }
    public float FinalTickRate { get; private set; }
    public int FinalCount { get; private set; }

    // IMPACT AREA
    public float BaseImpactRadius;
    public float PercentImpactRadius;
    public float FinalImpactRadius { get; private set; }

    public void Calculate()
    {
        // DAMAGE
        FinalDamage =
            (BaseDamage + FlatDamage)
            * (1f + PercentDamage)
            * FinalDamageMultiplier;

        // COOLDOWN
        float cooldownAfterFlat =
            BaseCooldown - FlatCooldownReduction;

        float cooldownAfterPercent =
            cooldownAfterFlat * (1f - PercentCooldownReduction);

        FinalCooldown = Mathf.Max(0.05f, cooldownAfterPercent);

        // AREA / RANGE / DURATION
        FinalSpawnRadius = Mathf.Max(0f, BaseSpawnRadius * (1f + PercentSpawnRadius));
        FinalImpactRadius = Mathf.Max(0f, BaseImpactRadius * (1f + PercentImpactRadius));
        FinalRange = Mathf.Max(0f, BaseRange * (1f + PercentRange));
        FinalDuration = Mathf.Max(0f, BaseDuration * (1f + PercentDuration));

        // TICK RATE
        FinalTickRate = Mathf.Max(0.01f, BaseTickRate * (1f + PercentTickRate));

        // COUNT
        FinalCount = Mathf.Max(0, BaseCount + ExtraCount);
    }
}