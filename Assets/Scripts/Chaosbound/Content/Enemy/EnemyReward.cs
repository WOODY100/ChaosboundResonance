#if CHAOSBOUND_LEGACY_ENEMY
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [Header("Rewards")]
    [SerializeField] private int experienceReward = 5;
    [SerializeField] private int goldReward = 1;

    [Header("Prefabs")]
    [SerializeField] private GameObject experienceOrbPrefab;

    private EnemyHealth health;

    [SerializeField]
    private LayerMask groundLayer;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        health.OnDeath += GiveReward;
    }

    private void OnDisable()
    {
        health.OnDeath -= GiveReward;
    }

    public void SetRewards(int xp, int gold)
    {
        experienceReward = xp;
        goldReward = gold;
    }

    private void GiveReward(EnemyHealth enemy)
    {
        SpawnXPOrb();
    }

    private void SpawnXPOrb()
    {
        if (experienceOrbPrefab == null)
            return;

        Vector3 spawnPosition = GetSpawnPosition();

        ResonanceFragmentPickup pickup =
            PoolManager.Instance.Get<ResonanceFragmentPickup>(
                experienceOrbPrefab,
                spawnPosition,
                experienceOrbPrefab.transform.rotation
            );

        if (pickup != null)
        {
            pickup.Initialize(experienceReward);

            ExpeditionRuntimeState runtimeState =
                RunManager.Instance
                    .ExpeditionDirector
                    .RuntimeState;

            if (runtimeState != null)
            {
                runtimeState
                    .XPFragments
                    .Register(pickup);
            }
        }
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 5f;

        if (Physics.Raycast(rayOrigin,
                            Vector3.down,
                            out RaycastHit hit,
                            20f,
                            groundLayer))
        {
            return hit.point + Vector3.up * 0.25f;
        }

        Vector3 fallback = transform.position;
        fallback.y = 0.5f;

        return fallback;
    }
}
#endif