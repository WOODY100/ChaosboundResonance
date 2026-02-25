using UnityEngine;

public class FloatingDamageManager : MonoBehaviour
{
    public static FloatingDamageManager Instance;

    [SerializeField] private GameObject floatingTextPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDamage(Vector3 position, float damage, bool isCritical)
    {
        if (floatingTextPrefab == null)
            return;

        Vector3 spawnPosition = position + Vector3.up * 1.5f;

        GameObject obj = Instantiate(
            floatingTextPrefab,
            spawnPosition,
            Quaternion.identity
        );

        FloatingDamageText text =
            obj.GetComponent<FloatingDamageText>();

        if (text != null)
        {
            text.Initialize(damage, isCritical);
        }
    }
}