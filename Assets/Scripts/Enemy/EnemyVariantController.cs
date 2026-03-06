using System.Collections;
using UnityEngine;

public class EnemyVariantController : MonoBehaviour
{
    [SerializeField] private EnemyVariantData variantData;

    private EnemyStats stats;
    private EnemyReward reward;
    private EnemyMovementArena movement;

    // 🔒 Temporal: control manual del modelo dinámico
    [SerializeField] private bool useDynamicModel = false;
    [SerializeField] private Transform modelContainer;

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
        if (variantData.modelPrefab == null || modelContainer == null)
            return;

        foreach (Transform child in modelContainer)
            Destroy(child.gameObject);

        GameObject model = Instantiate(
            variantData.modelPrefab,
            modelContainer
        );

        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;

        AutoFitCollider(model);

        StartCoroutine(RebindAnimatorNextFrame());
    }

    IEnumerator RebindAnimatorNextFrame()
    {
        yield return null;

        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }

    void AutoFitCollider(GameObject model)
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule == null)
            return;

        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        // altura total
        float height = bounds.size.y;

        // radio basado en el ancho
        float radius = Mathf.Max(bounds.size.x, bounds.size.z) * 0.15f;

        capsule.height = height;
        capsule.radius = radius;

        // centro relativo al Enemy_Base
        Vector3 center = transform.InverseTransformPoint(bounds.center);
        capsule.center = center;
    }
}
