/// <summary>
/// Represents any object capable of receiving damage.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Applies damage to this object.
    /// </summary>
    void TakeDamage(DamageData damage);

    /// <summary>
    /// Returns whether this object can no longer receive damage.
    /// </summary>
    bool IsDead { get; }
}