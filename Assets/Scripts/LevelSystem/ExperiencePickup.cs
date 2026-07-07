using UnityEngine;

public class ExperiencePickup : MonoBehaviour
{
    [Header("XP")]
    [SerializeField] private int xpAmount = 5;

    [Header("Movement")]
    [SerializeField] private float attractSpeed = 10f;
    [SerializeField] private float absorbDuration = 0.2f;

    private Transform player;
    private PlayerStats playerStats;
    private PlayerExperienceSystem xpSystem;
    private FloatingPickup floatingPickup;
    private PooledObject pooledObject;

    private bool isAttracted;
    private bool isAbsorbing;
    private float absorbTimer;
    private Vector3 startScale;

    public void Initialize(int xp)
    {
        xpAmount = xp;
        ResetState();
        EnsureReferences();
    }

    private void Awake()
    {
        startScale = transform.localScale;
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

        float attractRadius = playerStats != null
            ? playerStats.ExpAttractionRadius
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
            playerStats = playerXP.GetComponent<PlayerStats>();

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
    }

    private void AbsorbEffect()
    {
        if (player == null)
            return;

        absorbTimer += Time.deltaTime;

        float t = Mathf.Clamp01(absorbTimer / absorbDuration);
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
    }

    private void EnsureReferences()
    {
        if (xpSystem != null)
            return;

        xpSystem = FindFirstObjectByType<PlayerExperienceSystem>();

        if (xpSystem == null)
            return;

        player = xpSystem.transform;
        playerStats = xpSystem.GetComponent<PlayerStats>();
    }
}