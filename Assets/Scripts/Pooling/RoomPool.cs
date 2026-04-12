using System.Collections.Generic;
using UnityEngine;

public class RoomPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int initialSize = 5;
    }

    public List<Pool> pools;

    Dictionary<GameObject, Queue<GameObject>> poolDict = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        foreach (var p in pools)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < p.initialSize; i++)
            {
                GameObject obj = Instantiate(p.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            poolDict[p.prefab] = queue;
        }
    }

    public GameObject Get(GameObject prefab)
    {
        if (!poolDict.ContainsKey(prefab))
        {
            Debug.LogError($"[POOL ERROR] Prefab no registrado: {prefab.name}");
            poolDict[prefab] = new Queue<GameObject>();
        }

        var queue = poolDict[prefab];

        GameObject obj;

        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);
        }

        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj, GameObject prefab)
    {
        obj.SetActive(false);

        if (!poolDict.ContainsKey(prefab))
        {
            poolDict[prefab] = new Queue<GameObject>();
        }

        poolDict[prefab].Enqueue(obj);
    }
}