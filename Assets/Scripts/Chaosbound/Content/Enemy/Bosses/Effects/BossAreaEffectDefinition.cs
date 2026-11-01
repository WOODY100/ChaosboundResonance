using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Bosses/Effects/Area Effect",
        fileName = "BossAreaEffect")]
    public sealed class BossAreaEffectDefinition : BossAttackEffectDefinition
    {
        [SerializeField] private GameObject m_EffectPrefab;

        [Header("Telegraph")]
        [SerializeField] private GameObject m_TelegraphPrefab;

        [Header("Impact")]
        [SerializeField, Min(0.1f)] private float m_Radius = 3f;
        [SerializeField, Min(0.01f)] private float m_VisualDuration = 1.5f;
        [SerializeField, Min(0.01f)] private float m_DamageMultiplier = 1f;
        [SerializeField] private Vector3 m_PositionOffset = Vector3.zero;

        public GameObject EffectPrefab => m_EffectPrefab;
        public GameObject TelegraphPrefab => m_TelegraphPrefab;
        public float Radius => m_Radius;
        public float VisualDuration => m_VisualDuration;
        public float DamageMultiplier => m_DamageMultiplier;
        public Vector3 PositionOffset => m_PositionOffset;

        public bool Validate(out string error)
        {
            if (m_EffectPrefab == null)
                return Fail("EffectPrefab is required.", out error);
            if (m_EffectPrefab.GetComponent<PooledObject>() == null)
                return Fail("EffectPrefab requires PooledObject on its root.", out error);
            if (m_EffectPrefab.GetComponent<BossAreaEffectVisual>() == null)
                return Fail("EffectPrefab requires BossAreaEffectVisual on its root.", out error);

            if (m_TelegraphPrefab != null)
            {
                if (m_TelegraphPrefab.GetComponent<PooledObject>() == null)
                    return Fail("TelegraphPrefab requires PooledObject on its root.", out error);

                if (m_TelegraphPrefab.GetComponent<BossAreaTelegraphVisual>() == null)
                    return Fail("TelegraphPrefab requires BossAreaTelegraphVisual on its root.", out error);
            }

            if (m_Radius <= 0f || m_VisualDuration <= 0f || m_DamageMultiplier <= 0f)
                return Fail("Radius, VisualDuration and DamageMultiplier must be > 0.", out error);

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
                Debug.LogWarning($"BossAreaEffectDefinition '{name}': {error}", this);
        }
    }
}
