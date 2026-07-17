using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerDamageReceiver : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float globalDamageCooldown = 0.5f;

    public bool IsInvulnerable { get; set; }

    private PlayerHealth health;
    private float lastDamageTime;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();

        if (health == null)
            Debug.LogError($"{name} requires PlayerHealth.");
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
        return damageData.amount;
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