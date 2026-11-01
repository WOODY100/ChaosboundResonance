using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Bosses/Effects/Projectile Effect",
        fileName = "BossProjectileEffect")]
    public sealed class BossProjectileEffectDefinition : BossAttackEffectDefinition
    {
        [SerializeField] private GameObject m_ProjectilePrefab;
        [SerializeField, Min(0.01f)] private float m_Speed = 12f;
        [SerializeField, Min(0.1f)] private float m_MaxTravelDistance = 12f;
        [SerializeField, Min(0.01f)] private float m_DamageMultiplier = 1f;
        [SerializeField] private Vector3 m_LocalSpawnOffset = new Vector3(0f, 1.2f, 0.35f);

        public GameObject ProjectilePrefab => m_ProjectilePrefab;
        public float Speed => m_Speed;
        public float MaxTravelDistance => m_MaxTravelDistance;
        public float DamageMultiplier => m_DamageMultiplier;
        public Vector3 LocalSpawnOffset => m_LocalSpawnOffset;

        public bool Validate(out string error)
        {
            if (m_ProjectilePrefab == null)
                return Fail("ProjectilePrefab is required.", out error);
            if (m_ProjectilePrefab.GetComponent<PooledObject>() == null)
                return Fail("ProjectilePrefab requires PooledObject on its root.", out error);
            if (m_ProjectilePrefab.GetComponent<BossProjectileBehaviour>() == null)
                return Fail("ProjectilePrefab requires BossProjectileBehaviour on its root.", out error);
            if (m_ProjectilePrefab.GetComponent<Rigidbody>() == null)
                return Fail("ProjectilePrefab requires Rigidbody on its root.", out error);
            if (m_ProjectilePrefab.GetComponent<Collider>() == null)
                return Fail("ProjectilePrefab requires a Collider on its root.", out error);
            if (m_Speed <= 0f || m_MaxTravelDistance <= 0f || m_DamageMultiplier <= 0f)
                return Fail("Speed, MaxTravelDistance and DamageMultiplier must be > 0.", out error);

            error = null;
            return true;
        }

        private static bool Fail(string message, out string error)
        {
            error = message;
            return false;
        }

        private void OnValidate()
        {
            if (!Validate(out string error))
                Debug.LogWarning($"BossProjectileEffectDefinition '{name}': {error}", this);
        }
    }
}
