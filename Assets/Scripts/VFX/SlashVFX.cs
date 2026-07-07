using System.Collections;
using UnityEngine;

public class SlashVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem slashAdd;
    [SerializeField] private ParticleSystem slashAlp;
    [SerializeField] private ParticleSystem dust;

    private ParticleSystem[] particles;

    private void Awake()
    {
        particles = new ParticleSystem[3];

        particles[0] = slashAdd;
        particles[1] = slashAlp;
        particles[2] = dust;
    }

    private void OnEnable()
    {
        foreach (ParticleSystem ps in particles)
        {
            ps.Clear(true);
            ps.Play(true);
        }
    }

    public void SetColor(Color color)
    {
        SetCustomDataColor(slashAdd, color);
        SetCustomDataColor(slashAlp, color);
        SetStartColor(dust, color * 0.7f); // un poco más oscuro para profundidad
    }

    void SetCustomDataColor(ParticleSystem ps, Color color)
    {
        if (ps == null) return;

        var customData = ps.customData;
        customData.SetColor(ParticleSystemCustomData.Custom2, color);
    }

    void SetStartColor(ParticleSystem ps, Color color)
    {
        if (ps == null) return;

        var main = ps.main;
        main.startColor = color;
    }
}
