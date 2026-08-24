using UnityEngine;

public abstract class PooledBehaviour : MonoBehaviour
{
    private PooledObject pooledObject;

    protected virtual void Awake()
    {
        pooledObject = GetComponent<PooledObject>();
    }

    protected virtual void OnEnable()
    {
        ResetPooledState();
    }

    protected virtual void OnDisable()
    {
    }

    public void ReturnToPool()
    {
        if (pooledObject == null)
            pooledObject = GetComponent<PooledObject>();

        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Se ejecuta cada vez que el objeto sale del pool.
    /// </summary>
    protected virtual void ResetPooledState()
    {
    }
}