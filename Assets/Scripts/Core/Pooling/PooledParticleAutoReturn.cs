using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PooledParticleAutoReturn : MonoBehaviour
{
    [SerializeField] private float fallbackLifetime = 2f;
    [SerializeField] private bool waitForAllChildren = true;

    private ParticleSystem ps;
    private PooledObject pooledObject;
    private float timer;
    private bool isReturning;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        pooledObject = GetComponent<PooledObject>();
    }

    private void OnEnable()
    {
        timer = 0f;
        isReturning = false;

        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Clear(true);
            ps.Play(true);
        }
    }

    private void Update()
    {
        if (isReturning || ps == null)
            return;

        timer += Time.deltaTime;

        if (!ps.IsAlive(waitForAllChildren))
        {
            Return();
            return;
        }

        if (timer >= fallbackLifetime)
        {
            Return();
        }
    }

    private void OnParticleSystemStopped()
    {
        if (isReturning)
            return;

        if (ps != null && ps.IsAlive(waitForAllChildren))
            return;

        Return();
    }

    private void Return()
    {
        if (isReturning)
            return;

        isReturning = true;

        if (pooledObject == null)
            pooledObject = GetComponent<PooledObject>();

        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else
            gameObject.SetActive(false);
    }
}