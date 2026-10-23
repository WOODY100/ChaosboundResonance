using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerDamageReceiver : MonoBehaviour, IDamageable
{
    [Header("Damage Settings")]
    [SerializeField] private float globalDamageCooldown = 0.5f;

    [Header("Damage Audio")]
    [SerializeField] private AudioSource damageAudioSource;
    [SerializeField] private AudioClip damageSound;

    [SerializeField, Range(0f, 1f)]
    private float damageVolume = 0.8f;

    private PlayerModifierSystem modifierSystem;

    public bool IsInvulnerable { get; set; }

    public bool IsDead =>
    health == null || health.CurrentHealth <= 0f;

    public void TakeDamage(DamageData damageData)
    {
        ReceiveDamage(damageData);
    }

    private PlayerHealth health;
    private float lastDamageTime;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        modifierSystem = GetComponent<PlayerModifierSystem>();

        if (damageAudioSource == null)
        {
            damageAudioSource =
                GetComponent<AudioSource>();
        }

        if (health == null)
            Debug.LogError($"{name} requires PlayerHealth.");

        if (modifierSystem == null)
            Debug.LogError($"{name} requires PlayerModifierSystem.");
    }

    public void ReceiveDamage(DamageData damageData)
    {
        if (health == null)
            return;

        if (IsInvulnerable)
            return;

        if (!CanReceiveDamage())
            return;

        lastDamageTime = Time.time;

        ApplyDamage(damageData);

        // Solo reproducir si el jugador sigue vivo.
        if (health != null &&
            !health.IsDead &&
            damageAudioSource != null &&
            damageSound != null)
        {
            damageAudioSource.PlayOneShot(
                damageSound,
                damageVolume);
        }
    }

    private void ApplyDamage(DamageData damageData)
    {
        if (health == null)
            return;

        float finalDamage = CalculateFinalDamage(damageData);

        health.TakeDamage(finalDamage);
    }

    private float CalculateFinalDamage(DamageData damageData)
    {
        if (modifierSystem == null)
            return damageData.amount;

        float damageReduction =
            modifierSystem.GetStat(
                StatType.DamageReduction);

        damageReduction =
            Mathf.Clamp01(damageReduction);

        return damageData.amount *
               (1f - damageReduction);
    }

    private bool CanReceiveDamage()
    {
        return Time.time >= lastDamageTime + globalDamageCooldown;
    }

    public void ResetDamageCooldown()
    {
        lastDamageTime = 0f;
    }

    private void OnValidate()
    {
        globalDamageCooldown = Mathf.Max(0f, globalDamageCooldown);
    }
}