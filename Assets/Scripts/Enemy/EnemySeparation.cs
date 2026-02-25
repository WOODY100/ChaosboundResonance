using UnityEngine;

public class EnemySeparation : MonoBehaviour
{
    [Header("Separation Settings")]
    [SerializeField] private float separationRadius = 1.2f;
    [SerializeField] private float separationStrength = 2f;
    [SerializeField] private LayerMask enemyLayer;

    // Buffer estático reutilizable (sin GC)
    private static readonly Collider[] results = new Collider[16];

    private Transform cachedTransform;

    void Awake()
    {
        cachedTransform = transform;
    }

    public Vector3 CalculateSeparation()
    {
        int count = Physics.OverlapSphereNonAlloc(
            cachedTransform.position,
            separationRadius,
            results,
            enemyLayer
        );

        if (count == 0)
            return Vector3.zero;

        Vector3 separation = Vector3.zero;
        Vector3 currentPos = cachedTransform.position;

        for (int i = 0; i < count; i++)
        {
            Collider col = results[i];

            if (col == null)
                continue;

            if (col.transform == cachedTransform)
                continue;

            Vector3 diff = currentPos - col.transform.position;
            float sqrDist = diff.sqrMagnitude;

            if (sqrDist > 0.0001f)
            {
                separation += diff / sqrDist;
            }
        }

        return separation * separationStrength;
    }
}