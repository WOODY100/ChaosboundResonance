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

        Vector3 spawnPosition = transform.position;

        // Lanzamos raycast hacia abajo para encontrar el suelo
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition + Vector3.up * 2f,
                            Vector3.down,
                            out hit,
                            5f))
        {
            spawnPosition = hit.point + Vector3.up * 0.3f; // pequeño offset
        }

        GameObject orb = Instantiate(
            experienceOrbPrefab,
            spawnPosition,
            Quaternion.identity
        );

        ExperiencePickup pickup =
            orb.GetComponent<ExperiencePickup>();

        pickup?.Initialize(experienceReward);
    }
}