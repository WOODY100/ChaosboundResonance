using System.Collections.Generic;
using UnityEngine;

public interface IEnemyTickable
{
    void Tick();
}

public class EnemyTickManager : MonoBehaviour
{
    public static EnemyTickManager Instance;

    [Header("Tick Settings")]
    [SerializeField] private int ticksPerFrame = 20;

    private readonly List<IEnemyTickable> tickables = new List<IEnemyTickable>();

    private int currentIndex;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Register(IEnemyTickable tickable)
    {
        if (!tickables.Contains(tickable))
            tickables.Add(tickable);
    }

    public void Unregister(IEnemyTickable tickable)
    {
        int index = tickables.IndexOf(tickable);

        if (index >= 0)
        {
            tickables.RemoveAt(index);

            if (currentIndex >= tickables.Count)
                currentIndex = 0;
        }
    }

    void Update()
    {
        int total = tickables.Count;

        if (total == 0)
            return;

        int executions = Mathf.Min(ticksPerFrame, total);

        for (int i = 0; i < executions; i++)
        {
            if (currentIndex >= total)
                currentIndex = 0;

            tickables[currentIndex].Tick();
            currentIndex++;
        }
    }
}