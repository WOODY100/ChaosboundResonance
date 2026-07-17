using UnityEngine;

public class ExperiencePickup : MonoBehaviour
{
    [Header("XP")]
    [SerializeField] private int xpAmount = 5;

    [Header("Movement")]
    [SerializeField] private float attractSpeed = 10f;
    [SerializeField] private float absorbDuration = 0.2f;

    private Transform player;
    private PlayerModifierSystem modifierSystem;
    private PlayerExperienceSystem xpSystem;
    private FloatingPickup floatingPickup;
    private PooledObject pooledObject;

    private bool isAttracted;
    private bool isAbsorbing;
    private float absorbTimer;
    private Vector3 startScale;

    public void Initialize(int xp)
    {
        xpAmount = Mathf.Max(0, xp);

        ResetState();
        EnsureReferences();
    }

    private void Awake()
    {
        startScale = transform.localScale;

        if (startScale == Vector3.zero)
            startScale = Vector3.one;

        floatingPickup = GetComponent<FloatingPickup>();
        pooledObject = GetComponent<PooledObject>();
    }

    private void OnEnable()
    {
        ResetState();
        EnsureReferences();
    }

    private void Update()
    {
        if (player == null)
        {
            EnsureReferences();

            if (player == null)
                return;
        }

        if (isAbsorbing)
        {
            AbsorbEffect();
            return;
        }

        float attractRadius = modifierSystem != null
            ? modifierSystem.GetStat(StatType.ExpAttractionRadius)
            : 2f;

        float sqrDistance = (transform.position - player.position).sqrMagnitude;
        float sqrAttractRadius = attractRadius * attractRadius;

        if (sqrDistance <= sqrAttractRadius && !isAttracted)
        {
            isAttracted = true;

            if (floatingPickup != null)
                floatingPickup.DisableFloating();
        }

        if (isAttracted)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                attractSpeed * Time.deltaTime
            );

            sqrDistance =
                (transform.position - player.position).sqrMagnitude;
        }

        if (sqrDistance <= 0.09f)
        {
            StartAbsorb();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAbsorbing)
            return;

        PlayerExperienceSystem playerXP = other.GetComponent<PlayerExperienceSystem>();

        if (playerXP != null)
        {
            xpSystem = playerXP;
            player = playerXP.transform;
            modifierSystem = playerXP.GetComponent<PlayerModifierSystem>();

            StartAbsorb();
        }
    }

    public void Attract()
    {
        isAttracted = true;

        if (floatingPickup != null)
            floatingPickup.DisableFloating();
    }

    private void StartAbsorb()
    {
        if (isAbsorbing)
            return;

        isAbsorbing = true;
        absorbTimer = 0f;

        if (floatingPickup != null)
            floatingPickup.DisableFloating();
    }

    private void AbsorbEffect()
    {
        if (player == null)
            return;

        absorbTimer += Time.deltaTime;

        float duration = Mathf.Max(0.01f, absorbDuration);

        float t = Mathf.Clamp01(absorbTimer / duration);

        float curved = t * t;

        transform.position = Vector3.Lerp(
            transform.position,
            player.position,
            curved * 6f
        );

        transform.localScale = Vector3.Lerp(
            startScale,
            Vector3.zero,
            curved
        );

        if (absorbTimer >= absorbDuration)
        {
            GiveXP();
            ReturnToPool();
        }
    }

    private void GiveXP()
    {
        if (xpSystem == null && player != null)
            xpSystem = player.GetComponent<PlayerExperienceSystem>();

        xpSystem?.AddXP(xpAmount);
    }

    private void ReturnToPool()
    {
        if (pooledObject == null)
            pooledObject = GetComponent<PooledObject>();

        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else
            Destroy(gameObject);
    }

    private void ResetState()
    {
        isAttracted = false;
        isAbsorbing = false;
        absorbTimer = 0f;
        transform.localScale = startScale;
        transform.localRotation = Quaternion.identity;
        transform.SetParent(null, true);
    }

    private void EnsureReferences()
    {
        if (xpSystem != null)
            return;

        xpSystem = FindFirstObjectByType<PlayerExperienceSystem>();

        if (xpSystem == null)
            return;

        player = xpSystem.transform;
        modifierSystem = xpSystem.GetComponent<PlayerModifierSystem>();
    }

    private void OnValidate()
    {
        xpAmount = Mathf.Max(0, xpAmount);

        attractSpeed = Mathf.Max(0f, attractSpeed);

        absorbDuration = Mathf.Max(0.01f, absorbDuration);
    }
}