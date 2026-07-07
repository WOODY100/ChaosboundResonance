using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private PoolManager poolManager;
    private GameObject prefab;
    private bool isReturned;

    public GameObject Prefab => prefab;

    public void Initialize(GameObject originalPrefab, PoolManager owner)
    {
        prefab = originalPrefab;
        poolManager = owner;
    }

    private void OnEnable()
    {
        isReturned = false;
    }

    public void ReturnToPool()
    {
        if (isReturned)
            return;

        isReturned = true;

        if (poolManager != null)
            poolManager.Return(gameObject);
        else
            Destroy(gameObject);
    }
}