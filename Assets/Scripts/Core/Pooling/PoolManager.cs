using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        [Min(0)] public int initialSize = 20;
        [Min(1)] public int expandAmount = 10;
    }

    [System.Serializable]
    private class PoolStats
    {
        public string prefabName;
        public int active;
        public int inactive;
        public int total;
    }

    [Header("Optional Preloaded Pools")]
    [SerializeField] private List<PoolEntry> pools = new();

    [Header("Runtime Defaults")]
    [SerializeField] private int defaultInitialSize = 0;
    [SerializeField] private int defaultExpandAmount = 10;

    [Header("Runtime Stats")]
    [SerializeField] private bool showStats = true;
    [SerializeField] private List<PoolStats> runtimeStats = new();

    private readonly Dictionary<GameObject, Queue<GameObject>> poolsByPrefab = new();
    private readonly Dictionary<GameObject, GameObject> prefabByInstance = new();
    private readonly Dictionary<GameObject, int> expandAmountByPrefab = new();
    private readonly Dictionary<GameObject, int> totalCreatedByPrefab = new();
    private readonly Dictionary<GameObject, int> activeCountByPrefab = new();
    private readonly Dictionary<GameObject, Transform> parentByPrefab = new();
    private readonly Dictionary<GameObject, string> prefabNameByPrefab = new();
    private readonly Dictionary<GameObject, Vector3> defaultScaleByPrefab = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (PoolEntry entry in pools)
        {
            CreatePool(entry.prefab, entry.initialSize, entry.expandAmount);
        }

        RefreshStats();
    }

    private void CreatePool(GameObject prefab, int initialSize, int expandAmount)
    {
        if (prefab == null)
            return;

        if (poolsByPrefab.ContainsKey(prefab))
            return;

        Queue<GameObject> queue = new();

        poolsByPrefab.Add(prefab, queue);
        expandAmountByPrefab.Add(prefab, Mathf.Max(1, expandAmount));
        totalCreatedByPrefab.Add(prefab, 0);
        activeCountByPrefab.Add(prefab, 0);
        defaultScaleByPrefab.Add(prefab, prefab.transform.localScale);
        prefabNameByPrefab.Add(prefab, prefab.name);
        parentByPrefab.Add(prefab, CreatePoolParent(prefab));

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateInstance(prefab);
            queue.Enqueue(obj);
        }

        RefreshStats();
    }

    private Transform CreatePoolParent(GameObject prefab)
    {
        string prefabName = prefabNameByPrefab[prefab];

        GameObject parent = new GameObject(prefabName + "_Pool");
        parent.transform.SetParent(transform);
        parent.transform.localPosition = Vector3.zero;
        parent.transform.localRotation = Quaternion.identity;
        parent.transform.localScale = Vector3.one;

        return parent.transform;
    }

    private GameObject CreateInstance(GameObject prefab)
    {
        Transform poolParent = parentByPrefab[prefab];

        GameObject obj = Instantiate(prefab, poolParent);
        obj.SetActive(false);

        PooledObject pooledObject = obj.GetComponent<PooledObject>();

        if (pooledObject == null)
        {
            Debug.LogError($"Prefab '{prefab.name}' must have a PooledObject component.");
        }
        else
        {
            pooledObject.Initialize(prefab, this);
        }

        prefabByInstance[obj] = prefab;
        totalCreatedByPrefab[prefab]++;

        return obj;
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
            return null;

        if (!poolsByPrefab.ContainsKey(prefab))
            CreatePool(prefab, defaultInitialSize, defaultExpandAmount);

        Queue<GameObject> queue = poolsByPrefab[prefab];

        if (queue.Count == 0)
            ExpandPool(prefab);

        GameObject pooledObj = queue.Dequeue();

        activeCountByPrefab[prefab]++;

        Transform pooledTransform = pooledObj.transform;

        pooledTransform.SetParent(parentByPrefab[prefab]);
        pooledTransform.localScale = defaultScaleByPrefab[prefab];
        pooledTransform.SetPositionAndRotation(position, rotation);

        pooledObj.SetActive(true);

        RefreshStats();

        return pooledObj;
    }

    public T Get<T>(GameObject prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        GameObject obj = Get(prefab, position, rotation);

        if (obj == null)
            return null;

        return obj.GetComponent<T>();
    }

    public void Return(GameObject obj)
    {
        if (obj == null)
            return;

        if (!prefabByInstance.TryGetValue(obj, out GameObject prefab))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);

        Transform objTransform = obj.transform;
        objTransform.SetParent(parentByPrefab[prefab]);
        objTransform.localPosition = Vector3.zero;
        objTransform.localRotation = Quaternion.identity;
        objTransform.localScale = defaultScaleByPrefab[prefab];

        poolsByPrefab[prefab].Enqueue(obj);
        activeCountByPrefab[prefab] = Mathf.Max(0, activeCountByPrefab[prefab] - 1);

        RefreshStats();
    }

    private void ExpandPool(GameObject prefab)
    {
        int expandAmount = expandAmountByPrefab[prefab];

        for (int i = 0; i < expandAmount; i++)
        {
            GameObject obj = CreateInstance(prefab);
            poolsByPrefab[prefab].Enqueue(obj);
        }

        RefreshStats();
    }

    private void RefreshStats()
    {
        if (!showStats)
            return;

        runtimeStats.Clear();

        foreach (KeyValuePair<GameObject, Queue<GameObject>> pool in poolsByPrefab)
        {
            GameObject prefab = pool.Key;

            runtimeStats.Add(new PoolStats
            {
                prefabName = prefabNameByPrefab[prefab],
                active = activeCountByPrefab[prefab],
                inactive = pool.Value.Count,
                total = totalCreatedByPrefab[prefab]
            });
        }
    }
}