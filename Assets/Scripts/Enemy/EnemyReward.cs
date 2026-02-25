using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [SerializeField] private int experienceReward = 5;
    [SerializeField] private int goldReward = 1;
    [SerializeField] private GameObject experienceOrbPrefab;

    private EnemyHealth health;

    void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }

    void OnEnable()
    {
        health.OnDeath += GiveReward;
    }

    void OnDisable()
    {
        health.OnDeath -= GiveReward;
    }

    public void SetRewards(int xp, int gold)
    {
        experienceReward = xp;
        goldReward = gold;
    }

    void GiveReward(EnemyHealth enemy)
    {
        SpawnXPOrb();
    }

    private void SpawnXPOrb()
    {
        if (experienceOrbPrefab == null)
            return;

        GameObject orb = Instantiate(
            experienceOrbPrefab,
            transform.position,
            Quaternion.identity
        );

        ExperiencePickup pickup =
            orb.GetComponent<ExperiencePickup>();

        pickup?.Initialize(experienceReward);
    }
}