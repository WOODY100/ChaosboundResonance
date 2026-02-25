using System.Collections;
using UnityEngine;

public class SlashVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem slashAdd;
    [SerializeField] private ParticleSystem slashAlp;
    [SerializeField] private ParticleSystem dust;

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
