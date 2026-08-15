using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private float currentHealth;
    public bool IsDead => currentHealth <= 0f;

    private BossControllerBase controller;

    private void Awake()
    {
        currentHealth = maxHealth;
        controller = GetComponent<BossControllerBase>();
    }

    public void TakeDamage(DamageData damage)
    {
        if (currentHealth <= 0f)
            return;

        currentHealth -= damage.amount;

        // 🔥 Mostrar daño flotante
        if (FloatingDamageManager.Instance != null)
        {
            FloatingDamageManager.Instance.ShowDamage(
                transform.position,
                damage.amount,
                damage.isCrit
            );
        }

        controller?.OnHealthChanged(currentHealth / maxHealth);

        if (currentHealth <= 0f)
            controller?.OnDeath();
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}