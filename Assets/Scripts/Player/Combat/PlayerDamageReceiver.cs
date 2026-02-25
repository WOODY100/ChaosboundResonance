using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerDamageReceiver : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float globalDamageCooldown = 0.5f;

    private PlayerHealth health;
    private float lastDamageTime;

    void Awake()
    {
        health = GetComponent<PlayerHealth>();
    }

    public void ReceiveDamage(DamageData damageData)
    {
        if (Time.time < lastDamageTime + globalDamageCooldown)
            return;

        lastDamageTime = Time.time;

        ApplyDamage(damageData);
    }

    void ApplyDamage(DamageData damageData)
    {
        if (health == null)
            return;

        health.TakeDamage(damageData.amount);
    }
}