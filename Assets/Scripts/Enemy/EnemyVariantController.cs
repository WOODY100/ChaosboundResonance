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
        ApplyVariant();
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
                variantData.baseHealth,
                variantData.baseDamage,
                variantData.moveSpeed
            );
        }

        if (reward != null)
        {
            reward.SetRewards(
                variantData.experienceReward,
                variantData.goldReward
            );
        }

        if (movement != null)
        {
            movement.SetBaseSpeed(variantData.moveSpeed);
        }
    }
}