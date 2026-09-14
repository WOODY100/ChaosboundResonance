using UnityEngine;

public class XPVisualBenchmark : MonoBehaviour
{
    [Header("Benchmark")]
    [SerializeField] private GameObject xpPrefab;
    [SerializeField] private int count = 100;
    [SerializeField] private float spacing = 2.0f;

    [Header("Layout")]
    [SerializeField] private int columns = 0;

    private Transform spawnedRoot;

    private void Start()
    {
        Spawn();
    }

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        Clear();

        if (xpPrefab == null)
        {
            Debug.LogError("XPVisualBenchmark: XP Prefab is not assigned.");
            return;
        }

        spawnedRoot = new GameObject("Benchmark_XP_Instances").transform;
        spawnedRoot.SetParent(transform);

        int columnCount = columns > 0
            ? columns
            : Mathf.CeilToInt(Mathf.Sqrt(count));

        for (int i = 0; i < count; i++)
        {
            int x = i % columnCount;
            int z = i / columnCount;

            float center = (columnCount - 1) * 0.5f;

            Vector3 position = new Vector3(
                (x - center) * spacing,
                0.15f,
                (z - center) * spacing
            );

            Instantiate(
                xpPrefab,
                position,
                xpPrefab.transform.rotation,
                spawnedRoot
            );
        }

        Debug.Log(
            "XP Visual Benchmark spawned " +
            count +
            " instances."
        );
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (spawnedRoot == null)
            return;

        for (int i = spawnedRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(spawnedRoot.GetChild(i).gameObject);
        }

        Destroy(spawnedRoot.gameObject);
        spawnedRoot = null;
    }
}