using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyAbilityDefinition",
    menuName = "Chaosbound/Enemies/Ability Definition"
)]
public sealed class EnemyAbilityDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string contentId;
    [SerializeField] private string displayName;

    [Header("Execution")]
    [SerializeField] private EnemyAbilityExecutionType executionType;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileRangeExtension = 2f;

    public string ContentId =>
        contentId;

    public string DisplayName =>
        displayName;

    public EnemyAbilityExecutionType ExecutionType =>
        executionType;

    public GameObject ProjectilePrefab =>
        projectilePrefab;

    public float ProjectileSpeed =>
        projectileSpeed;

    public float ProjectileRangeExtension =>
        projectileRangeExtension;

    private void OnValidate()
    {
        projectileSpeed =
            Mathf.Max(0f, projectileSpeed);

        projectileRangeExtension =
            Mathf.Max(0f, projectileRangeExtension);
    }
}