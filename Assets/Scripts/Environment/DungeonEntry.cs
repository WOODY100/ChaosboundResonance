using UnityEngine;

public enum EntryDirection
{
    North,
    South,
    East,
    West
}

public class DungeonEntry : MonoBehaviour
{
    void Awake()
    {
        occupied = false;
    }

    public EntryDirection direction;
    public bool occupied;

    public Transform ParentRoom => transform.root;
}