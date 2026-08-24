using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Content.Portal.Exit;
using Chaosbound.Gameplay.Spawn.Integration;
using System;
using UnityEngine;

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
                throw new ArgumentNullException(
                    nameof(request));

            if (request.Reference is EnemyVariantData enemy)
            {
                return SpawnEnemy(
                    enemy,
                    request);
            }

            if (request.Reference is BossData boss)
            {
                return SpawnBoss(
                    boss,
                    request);
            }

            if (request.Reference is MiniBossData miniBoss)
            {
                return SpawnMiniBoss(
                    miniBoss,
                    request);
            }

            if (request.Reference is ExitPortalData exitPortal)
            {
                return SpawnExitPortal(
                    exitPortal,
                    request);
            }

            throw new InvalidOperationException(
                $"Unsupported materializable reference " +
                $"'{request.Reference.GetType().Name}'.");
        }

        private GameObject SpawnEnemy(
            EnemyVariantData enemy,
            SpawnInstantiationRequest request)
        {
            if (enemy.SpawnPrefab == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{enemy.name}' does not define a spawn prefab.");
            }

            GameObject instance =
                GetFromPool(
                    enemy.SpawnPrefab,
                    request);

            return instance;
        }

        private GameObject SpawnBoss(
            BossData boss,
            SpawnInstantiationRequest request)
        {
            if (boss.SpawnPrefab == null)
            {
                throw new InvalidOperationException(
                    $"Boss '{boss.name}' does not define a spawn prefab.");
            }

            return GetFromPool(
                boss.SpawnPrefab,
                request);
        }

        private GameObject SpawnMiniBoss(
            MiniBossData miniBoss,
            SpawnInstantiationRequest request)
        {
            if (miniBoss.SpawnPrefab == null)
            {
                throw new InvalidOperationException(
                    $"MiniBoss '{miniBoss.name}' does not define a spawn prefab.");
            }

            return GetFromPool(
                miniBoss.SpawnPrefab,
                request);
        }

        private GameObject SpawnExitPortal(
            ExitPortalData exitPortal,
            SpawnInstantiationRequest request)
        {
            if (exitPortal.SpawnPrefab == null)
            {
                throw new InvalidOperationException(
                    $"Exit Portal '{exitPortal.name}' " +
                    "does not define a spawn prefab.");
            }

            return GetFromPool(
                exitPortal.SpawnPrefab,
                request);
        }

        private GameObject GetFromPool(
            GameObject prefab,
            SpawnInstantiationRequest request)
        {
            if (PoolManager.Instance == null)
            {
                throw new InvalidOperationException(
                    "PoolManager.Instance is not available.");
            }

            return PoolManager.Instance.Get(
                prefab,
                request.Position,
                request.Rotation);
        }
    }
}