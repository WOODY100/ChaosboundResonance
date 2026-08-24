using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyProjectileTest : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Test")]
    [SerializeField] private Vector3 direction = Vector3.forward;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private DamageType damageType;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (PoolManager.Instance == null)
        {
            Debug.LogError(
                "EnemyProjectileTest could not find PoolManager.Instance.");

            return;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError(
                "EnemyProjectileTest has no projectile prefab assigned.");

            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError(
                "EnemyProjectileTest has no spawn point assigned.");

            return;
        }

        Vector3 fireDirection =
            direction.sqrMagnitude > 0.0001f
                ? direction.normalized
                : spawnPoint.forward;

        Quaternion rotation =
            Quaternion.LookRotation(fireDirection);

        GameObject projectileObject =
            PoolManager.Instance.Get(
                projectilePrefab,
                spawnPoint.position,
                rotation);

        if (projectileObject == null)
        {
            Debug.LogError(
                "EnemyProjectileTest failed to obtain projectile from PoolManager.");

            return;
        }

        EnemyProjectile projectile =
            projectileObject.GetComponent<EnemyProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                $"Prefab '{projectilePrefab.name}' does not contain EnemyProjectile.");

            return;
        }

        projectile.Initialize(
            transform,
            fireDirection,
            speed,
            lifetime,
            damage,
            damageType);

        Debug.Log(
            $"EnemyProjectileTest fired '{projectilePrefab.name}' " +
            $"direction={fireDirection} " +
            $"speed={speed} " +
            $"lifetime={lifetime}.");
    }
}