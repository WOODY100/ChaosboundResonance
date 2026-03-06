using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    public float DistanceToPlayer { get; private set; }

    public Transform Player => EnemyManager.Instance != null
        ? EnemyManager.Instance.Player
        : null;

    private CapsuleCollider enemyCollider;
    private CharacterController playerController;

    void Awake()
    {
        enemyCollider = GetComponent<CapsuleCollider>();
    }

    public void UpdateDistance()
    {
        if (Player == null)
            return;

        if (playerController == null)
            playerController = Player.GetComponent<CharacterController>();

        float centerDistance =
            (Player.position - transform.position).magnitude;

        float enemyRadius = enemyCollider.radius * transform.lossyScale.x;
        float playerRadius = playerController.radius;

        DistanceToPlayer = Mathf.Max(0, centerDistance - enemyRadius - playerRadius);
    }
}