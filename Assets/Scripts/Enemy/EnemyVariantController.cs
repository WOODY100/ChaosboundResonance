using UnityEngine;

public class EnemyVariantController : MonoBehaviour
{
    [SerializeField] private EnemyVariantData variantData;

    private EnemyStats stats;
    private EnemyReward reward;
    private EnemyMovementArena movement;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        reward = GetComponent<EnemyReward>();
        movement = GetComponent<EnemyMovementArena>();
    }

    private void OnEnable()
    {
        if (variantData != null)
        {
            ApplyVariant();
        }
    }

    public void SetVariant(EnemyVariantData data)
    {
        variantData = data;
        ApplyVariant();
    }

    private void ApplyVariant()
    {
        if (variantData == null)
            return;

        if (stats != null)
        {
            stats.SetBaseStats(
                variantData.BaseHealth,
                variantData.BaseDamage,
                variantData.MoveSpeed
            );
        }

        if (reward != null)
        {
            reward.SetRewards(
                variantData.ExperienceReward,
                variantData.GoldReward
            );
        }

        if (movement != null)
        {
            movement.SetBaseSpeed(variantData.MoveSpeed);
        }
    }
}