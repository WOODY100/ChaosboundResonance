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
    public EntryDirection direction;
    public bool occupied;

    public Transform ParentRoom => transform.root;
}