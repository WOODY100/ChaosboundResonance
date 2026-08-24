using UnityEngine;

public class ResonanceFragmentPickup : PooledBehaviour
{
    [Header("XP")]
    [SerializeField] private int xpAmount = 5;

    [Header("Attraction")]
    [SerializeField] private float defaultAttractionRadius = 2f;
    [SerializeField] private float attractSpeed = 10f;

    [Header("Absorption")]
    [SerializeField] private float absorbDistance = 0.3f;
    [SerializeField] private float absorbDuration = 0.2f;

    private Transform player;
    private PlayerModifierSystem modifierSystem;
    private PlayerExperienceSystem xpSystem;

    private bool isAttracted;
    private bool isAbsorbing;

    private float absorbTimer;
    private Vector3 startScale;

    protected override void Awake()
    {
        base.Awake();

        startScale = transform.localScale;

        if (startScale == Vector3.zero)
            startScale = Vector3.one;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ResolvePlayer();
    }

    public void Initialize(int xp)
    {
        xpAmount = Mathf.Max(0, xp);

        ResetPooledState();
        ResolvePlayer();
    }

    private void Update()
    {
        if (player == null)
        {
            ResolvePlayer();

            if (player == null)
                return;
        }

        if (isAbsorbing)
        {
            UpdateAbsorption();
            return;
        }

        float attractionRadius = GetAttractionRadius();

        float sqrDistance =
            (transform.position - player.position).sqrMagnitude;

        if (!isAttracted &&
            sqrDistance <= attractionRadius * attractionRadius)
        {
            BeginAttraction();
        }

        if (!isAttracted)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            attractSpeed * Time.deltaTime
        );

        sqrDistance =
            (transform.position - player.position).sqrMagnitude;

        if (sqrDistance <= absorbDistance * absorbDistance)
        {
            StartAbsorption();
        }
    }

    public void Attract()
    {
        if (isAbsorbing)
            return;

        ResolvePlayer();

        if (player == null)
            return;

        BeginAttraction();
    }

    private void BeginAttraction()
    {
        isAttracted = true;
    }

    private void StartAbsorption()
    {
        if (isAbsorbing)
            return;

        isAbsorbing = true;
        absorbTimer = 0f;
    }

    private void UpdateAbsorption()
    {
        if (player == null)
        {
            ResolvePlayer();

            if (player == null)
                return;
        }

        absorbTimer += Time.deltaTime;

        float duration =
            Mathf.Max(0.01f, absorbDuration);

        float t =
            Mathf.Clamp01(
                absorbTimer / duration);

        float curved = t * t;

        transform.position = Vector3.Lerp(
            transform.position,
            player.position,
            curved
        );

        transform.localScale = Vector3.Lerp(
            startScale,
            Vector3.zero,
            curved
        );

        if (absorbTimer >= duration)
        {
            GiveXP();
            ReturnToPool();
        }
    }

    private void GiveXP()
    {
        if (xpSystem == null && player != null)
            xpSystem =
                player.GetComponent<PlayerExperienceSystem>();

        if (xpSystem != null)
            xpSystem.AddXP(xpAmount);
    }

    private float GetAttractionRadius()
    {
        if (modifierSystem != null)
        {
            return Mathf.Max(
                0f,
                modifierSystem.GetStat(
                    StatType.ExpAttractionRadius));
        }

        return defaultAttractionRadius;
    }

    private void ResolvePlayer()
    {
        if (player != null)
            return;

        if (EnemyManager.Instance == null)
            return;

        player =
            EnemyManager.Instance.Player;

        if (player == null)
            return;

        xpSystem =
            player.GetComponent<PlayerExperienceSystem>();

        modifierSystem =
            player.GetComponent<PlayerModifierSystem>();
    }

    protected override void ResetPooledState()
    {
        isAttracted = false;
        isAbsorbing = false;
        absorbTimer = 0f;

        transform.localScale = startScale;
    }

    private void OnValidate()
    {
        xpAmount =
            Mathf.Max(0, xpAmount);

        defaultAttractionRadius =
            Mathf.Max(
                0f,
                defaultAttractionRadius);

        attractSpeed =
            Mathf.Max(
                0f,
                attractSpeed);

        absorbDistance =
            Mathf.Max(
                0.01f,
                absorbDistance);

        absorbDuration =
            Mathf.Max(
                0.01f,
                absorbDuration);
    }

    public void Cleanup()
    {
        ReturnToPool();
    }
}