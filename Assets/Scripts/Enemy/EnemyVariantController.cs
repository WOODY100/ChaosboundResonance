using UnityEngine;

public class EnemyVariantController : MonoBehaviour
{
    [SerializeField] private EnemyVariantData variantData;

    private EnemyStats stats;
    private EnemyReward reward;
    private EnemyMovementArena movement;

    // 🔒 Temporal: control manual del modelo dinámico
    [SerializeField] private bool useDynamicModel = false;

    void Awake()
    {
        stats = GetComponent<EnemyStats>();
        reward = GetComponent<EnemyReward>();
        movement = GetComponent<EnemyMovementArena>();
    }

    void OnEnable()
    {
        ApplyVariant();
    }

    public void SetVariant(EnemyVariantData data)
    {
        variantData = data;
        ApplyVariant();
    }

    void ApplyVariant()
    {
        if (variantData == null)
            return;

        if (stats != null)
        {
            stats.SetBaseStats(
                variantData.baseHealth,
                variantData.baseDamage,
                variantData.moveSpeed
            );
        }

        if (reward != null)
        {
            reward.SetRewards(
                variantData.experienceReward,
                variantData.goldReward
            );
        }

        if (movement != null)
        {
            movement.SetBaseSpeed(variantData.moveSpeed);
        }

        // 🔒 Solo si activas dinámico
        if (useDynamicModel)
        {
            SetupModel();
        }
    }

    void SetupModel()
    {
        if (variantData.modelPrefab == null)
            return;

        // Busca el contenedor visual
        Transform modelContainer = transform.Find("EnemyModel");
        if (modelContainer == null)
            return;

        // Limpia hijos anteriores
        foreach (Transform child in modelContainer)
        {
            Destroy(child.gameObject);
        }

        GameObject model = Instantiate(
            variantData.modelPrefab,
            modelContainer
        );

        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
    }
}
