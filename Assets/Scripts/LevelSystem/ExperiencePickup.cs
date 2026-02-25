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
    private FloatingPickup floating;

    public void Initialize(int xp)
    {
        xpAmount = xp;
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerExperienceSystem>()?.transform;
        startScale = transform.localScale;
        floating = GetComponent<FloatingPickup>();
    }

    void Update()
    {
        if (isAbsorbing)
        {
            AbsorbEffect();
            return;
        }

        if (isAttracted && player != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                attractSpeed * Time.deltaTime
            );
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

        if (floating != null)
            floating.enabled = false;
    }

    private void StartAbsorb()
    {
        isAbsorbing = true;
        absorbTimer = 0f;

        if (floating != null)
            floating.enabled = false;
    }

    private void AbsorbEffect()
    {
        if (player == null)
            return;

        absorbTimer += Time.deltaTime;

        float t = absorbTimer / absorbDuration;

        // Movimiento rápido al centro
        transform.position = Vector3.Lerp(
            transform.position,
            player.position,
            t * 5f
        );

        // Escala hacia 0
        transform.localScale = Vector3.Lerp(
            startScale,
            Vector3.zero,
            t
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