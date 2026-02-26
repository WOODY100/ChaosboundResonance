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

        RaycastHit hit;

        // Lanzamos raycast desde arriba del enemigo
        Vector3 rayOrigin = transform.position + Vector3.up * 5f;

        if (Physics.Raycast(rayOrigin,
                            Vector3.down,
                            out hit,
                            20f,
                            LayerMask.GetMask("Ground"))) // 🔥 filtrar solo suelo
        {
            spawnPosition = hit.point + Vector3.up * 0.25f;
        }
        else
        {
            // Fallback por si no detecta suelo
            spawnPosition = transform.position;
            spawnPosition.y = 0.5f;
        }

        GameObject orb = Instantiate(
            experienceOrbPrefab,
            spawnPosition,
            Quaternion.identity
        );

        ExperiencePickup pickup = orb.GetComponent<ExperiencePickup>();
        pickup?.Initialize(experienceReward);
    }
}