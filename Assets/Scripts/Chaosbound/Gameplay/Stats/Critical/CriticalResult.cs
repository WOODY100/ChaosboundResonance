public readonly struct CriticalResult
{
    public bool IsCritical { get; }
    public float DamageMultiplier { get; }

    public CriticalResult(
        bool isCritical,
        float damageMultiplier)
    {
        IsCritical = isCritical;
        DamageMultiplier = damageMultiplier;
    }

    public static CriticalResult NoCritical =>
        new CriticalResult(
            false,
            1f);
}