using UnityEngine;

public class RoomBoundsDebug : MonoBehaviour
{
    public float size = 12;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(size, 0.1f, size));
    }
}