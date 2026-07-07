using UnityEngine;

public class FloatingDamageManager : MonoBehaviour
{
    public static FloatingDamageManager Instance { get; private set; }

    [SerializeField] private GameObject floatingTextPrefab;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatic()
    {
        Instance = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowDamage(Vector3 position, float damage, bool isCritical)
    {
        if (floatingTextPrefab == null)
            return;

        Vector3 spawnPosition = position + Vector3.up * 1.5f;

        FloatingDamageText text = PoolManager.Instance.Get<FloatingDamageText>(
            floatingTextPrefab,
            spawnPosition,
            Quaternion.identity
        );

        if (text != null)
            text.Initialize(damage, isCritical);
    }
}