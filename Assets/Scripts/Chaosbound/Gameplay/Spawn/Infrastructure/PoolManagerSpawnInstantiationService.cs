using System;
using UnityEngine;
using Chaosbound.Gameplay.Spawn.Integration;

namespace Chaosbound.Gameplay.Spawn.Infrastructure
{
    /// <summary>
    /// Instantiates gameplay entities using the PoolManager.
    /// </summary>
    public sealed class PoolManagerSpawnInstantiationService :
        ISpawnInstantiationService
    {
        public GameObject Spawn(
            SpawnInstantiationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Reference is not EnemyVariantData enemy)
            {
                throw new InvalidOperationException(
                    $"Unsupported materializable reference '{request.Reference.GetType().Name}'.");
            }

            if (enemy.SpawnPrefab == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{enemy.name}' does not define a spawn prefab.");
            }

            if (PoolManager.Instance == null)
            {
                throw new InvalidOperationException(
                    "PoolManager.Instance is not available.");
            }

            GameObject instance =
                PoolManager.Instance.Get(
                    enemy.SpawnPrefab,
                    request.Position,
                    request.Rotation);

            EnemyVariantController controller =
                instance.GetComponent<EnemyVariantController>();

            if (controller != null)
            {
                controller.SetVariant(enemy);
            }

            return instance;
        }
    }
}