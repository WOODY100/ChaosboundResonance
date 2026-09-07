using UnityEngine;

public sealed class ResonanceFragmentPickup
    : AutoPickupBehaviour, IResourcePickup
{
    [Header("XP")]
    [SerializeField]
    private int xpAmount = 5;

    [Header("Attraction")]
    [SerializeField]
    private float defaultAttractionRadius = 2f;

    [SerializeField]
    private float attractSpeed = 10f;

    [Header("Absorption")]
    [SerializeField]
    private float absorbDistance = 0.3f;

    [SerializeField]
    private float absorbDuration = 0.2f;

    private PlayerModifierSystem modifierSystem;
    private PlayerExperienceSystem xpSystem;

    private bool isAttracted;
    private bool isAbsorbing;

    private float absorbTimer;
    private Vector3 startScale;

    protected override void Awake()
    {
        base.Awake();

        startScale =
            transform.localScale;

        if (startScale == Vector3.zero)
            startScale = Vector3.one;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ResolvePlayerComponents();
    }

    public void Initialize(int xp)
    {
        xpAmount =
            Mathf.Max(0, xp);

        ResetPooledState();

        ResolvePlayerComponents();
    }

    protected override void OnAutoPickupTriggered()
    {
        BeginAttraction();
    }

    protected override void OnAutoPickupUpdate()
    {
        if (Player == null)
            return;

        if (isAbsorbing)
        {
            UpdateAbsorption();
            return;
        }

        if (!isAttracted)
            return;

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                Player.position,
                attractSpeed * Time.deltaTime);

        float sqrDistance =
            (transform.position - Player.position).sqrMagnitude;

        if (sqrDistance <= absorbDistance * absorbDistance)
        {
            StartAbsorption();
        }
    }

    public void Attract()
    {
        if (isAbsorbing)
            return;

        ResolvePlayerComponents();

        if (Player == null)
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
        if (Player == null)
        {
            ResolvePlayerComponents();

            if (Player == null)
                return;
        }

        absorbTimer +=
            Time.deltaTime;

        float duration =
            Mathf.Max(
                0.01f,
                absorbDuration);

        float t =
            Mathf.Clamp01(
                absorbTimer / duration);

        float curved =
            t * t;

        transform.position =
            Vector3.Lerp(
                transform.position,
                Player.position,
                curved);

        transform.localScale =
            Vector3.Lerp(
                startScale,
                Vector3.zero,
                curved);

        if (absorbTimer >= duration)
        {
            GiveXP();
            ReturnToPool();
        }
    }

    private void GiveXP()
    {
        if (xpSystem == null &&
            Player != null)
        {
            xpSystem =
                Player.GetComponent<PlayerExperienceSystem>();
        }

        if (xpSystem != null)
            xpSystem.AddXP(xpAmount);
    }

    protected override float GetPickupRadius()
    {
        return GetAttractionRadius();
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

    private void ResolvePlayerComponents()
    {
        ResolvePlayer();

        if (Player == null)
            return;

        if (xpSystem == null)
        {
            xpSystem =
                Player.GetComponent<PlayerExperienceSystem>();
        }

        if (modifierSystem == null)
        {
            modifierSystem =
                Player.GetComponent<PlayerModifierSystem>();
        }
    }

    protected override void ResetPooledState()
    {
        base.ResetPooledState();

        isAttracted = false;
        isAbsorbing = false;
        absorbTimer = 0f;

        transform.localScale =
            startScale;
    }

    private void OnValidate()
    {
        xpAmount =
            Mathf.Max(
                0,
                xpAmount);

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