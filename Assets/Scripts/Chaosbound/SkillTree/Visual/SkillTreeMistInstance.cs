using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public sealed class SkillTreeMistInstance : MonoBehaviour
{
    private Image image;
    private Material runtimeMaterial;

    private static readonly int PhaseProperty =
        Shader.PropertyToID("_Phase");

    private static readonly int ColorProperty =
    Shader.PropertyToID("_Color");

    private void Awake()
    {
        image = GetComponent<Image>();

        if (image == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeMistInstance)}] " +
                $"'{name}' requires an Image component.",
                this);

            return;
        }

        if (image.material == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeMistInstance)}] " +
                $"'{name}' requires a material.",
                this);

            return;
        }

        runtimeMaterial =
            new Material(image.material);

        runtimeMaterial.name =
            image.material.name +
            " (Runtime)";

        image.material =
            runtimeMaterial;
    }

    public void SetPhase(float phase)
    {
        if (runtimeMaterial == null)
            return;

        runtimeMaterial.SetFloat(
            PhaseProperty,
            phase);
    }

    public void SetColor(Color color)
    {
        if (runtimeMaterial == null)
            return;

        runtimeMaterial.SetColor(
            ColorProperty,
            color);
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
            runtimeMaterial = null;
        }
    }
}