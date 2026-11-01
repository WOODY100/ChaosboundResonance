using UnityEngine;

[RequireComponent(typeof(PooledObject))]
public sealed class BossAreaEffectVisual : MonoBehaviour
{
    private float remainingDuration;
    private bool initialized;

    private void OnEnable()
    {
        remainingDuration = 0f;
        initialized = false;
    }

    public bool Initialize(float duration)
    {
        if (duration <= 0f)
        {
            ReturnToPool();
            return false;
        }

        remainingDuration = duration;
        initialized = true;
        return true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        remainingDuration -= Time.deltaTime;
        if (remainingDuration <= 0f)
            ReturnToPool();
    }

    private void ReturnToPool()
    {
        PooledObject pooled = GetComponent<PooledObject>();
        if (pooled != null)
            pooled.ReturnToPool();
        else
            Destroy(gameObject);
    }
}
