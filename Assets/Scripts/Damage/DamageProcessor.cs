using UnityEngine;

public static class DamageProcessor
{
    public static float CalculateDamage(EnemyHealth enemyHealth, DamageData damageData)
    {
        float finalDamage = damageData.amount;

        switch (damageData.type)
        {
            case DamageType.Fire:
                finalDamage *= 1.2f;
                break;

            case DamageType.Chaos:
                finalDamage *= Random.Range(0.8f, 1.5f);
                break;

            case DamageType.Poison:
                // 🔒 Reservado para sistema futuro de DoT
                break;
        }

        return finalDamage;
    }
}