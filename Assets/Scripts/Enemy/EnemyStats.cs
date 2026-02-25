using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float baseHealth = 10f;
    [SerializeField] private float baseDamage = 5f;
    [SerializeField] private float moveSpeed = 3.5f;

    public float CurrentHealth { get; private set; }
    public float Damage => baseDamage;
    public float MoveSpeed => moveSpeed;

    public void Initialize(float healthMultiplier = 1f, float damageMultiplier = 1f)
    {
        CurrentHealth = baseHealth * healthMultiplier;
        baseDamage *= damageMultiplier;
    }

    public void SetBaseStats(float health, float damage, float speed)
    {
        baseHealth = health;
        baseDamage = damage;
        moveSpeed = speed;

        ResetStats();
    }

    public void ResetStats()
    {
        CurrentHealth = baseHealth;
    }
}