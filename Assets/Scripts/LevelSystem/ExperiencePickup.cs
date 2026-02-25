using Unity.VisualScripting;
using UnityEngine;

public class ExperiencePickup : MonoBehaviour
{
    [SerializeField] private int xpAmount = 5;
    [SerializeField] private float attractSpeed = 10f;
    [SerializeField] private float absorbDuration = 0.2f;

    private Transform player;
    private bool isAttracted;
    private bool isAbsorbing;
    private float absorbTimer;
    private Vector3 startScale;
    private PlayerStats playerStats;
    private float attractRadius;

    public void Initialize(int xp)
    {
        xpAmount = xp;
    }

    void Start()
    {
        var xpSystem = FindFirstObjectByType<PlayerExperienceSystem>();
        if (xpSystem != null)
        {
            player = xpSystem.transform;
            playerStats = player.GetComponent<PlayerStats>();
        }

        startScale = transform.localScale;
    }

    void Update()
    {
        if (player == null)
            return;

        if (isAbsorbing)
        {
            AbsorbEffect();
            return;
        }

        attractRadius = playerStats != null
            ? playerStats.ExpAttractionRadius
            : 2f;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= attractRadius && !isAttracted)
        {
            isAttracted = true;

            var floating = GetComponent<FloatingPickup>();
            if (floating != null)
                floating.DisableFloating();
        }

        if (isAttracted)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                attractSpeed * Time.deltaTime
            );
        }

        if (distance <= 0.3f)
        {
            StartAbsorb();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isAbsorbing)
            return;

        if (other.GetComponent<PlayerExperienceSystem>() != null)
        {
            StartAbsorb();
        }
    }

    public void Attract()
    {
        isAttracted = true;
    }

    private void StartAbsorb()
    {
        isAbsorbing = true; absorbTimer = 0f;
    }

    private void AbsorbEffect()
    {
        if (player == null)
            return;

        absorbTimer += Time.deltaTime;
        float t = absorbTimer / absorbDuration;

        // Curva más natural
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
            Destroy(gameObject);
        }
    }

    private void GiveXP()
    {
        var xpSystem = player.GetComponent<PlayerExperienceSystem>();
        xpSystem?.AddXP(xpAmount);
    }
}