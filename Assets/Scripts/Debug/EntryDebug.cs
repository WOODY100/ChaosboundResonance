using UnityEngine;

public class EntryDebug : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.4f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
    }
}