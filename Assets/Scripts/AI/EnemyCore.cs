using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    public float DistanceToPlayer { get; private set; }

    public Transform Player => EnemyManager.Instance != null
        ? EnemyManager.Instance.Player
        : null;

    public void UpdateDistance()
    {
        if (Player == null)
            return;

        DistanceToPlayer = (Player.position - transform.position).sqrMagnitude;
    }
}