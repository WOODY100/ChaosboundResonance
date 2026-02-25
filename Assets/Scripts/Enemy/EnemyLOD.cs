using UnityEngine;

public class EnemyLOD : MonoBehaviour
{
    public enum LODLevel
    {
        Full,
        Simple,
        Disabled
    }

    public LODLevel CurrentLOD { get; private set; }

    [Header("LOD Distances")]
    [SerializeField] private float fullDistance = 12f;
    [SerializeField] private float simpleDistance = 25f;

    private float sqrFullDistance;
    private float sqrSimpleDistance;

    private EnemyCore core;

    void Awake()
    {
        core = GetComponent<EnemyCore>();

        sqrFullDistance = fullDistance * fullDistance;
        sqrSimpleDistance = simpleDistance * simpleDistance;
    }

    public void UpdateLOD()
    {
        if (core.Player == null)
            return;

        float distance = core.DistanceToPlayer;

        if (distance <= sqrFullDistance)
        {
            CurrentLOD = LODLevel.Full;
        }
        else if (distance <= sqrSimpleDistance)
        {
            CurrentLOD = LODLevel.Simple;
        }
        else
        {
            CurrentLOD = LODLevel.Disabled;
        }
    }
}