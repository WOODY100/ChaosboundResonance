using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public sealed class HealthOrbFillAnimator : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField]
    private Color color = Color.red;

    [SerializeField, Range(0f, 3f)]
    private float brightness = 1f;

    [SerializeField, Range(0f, 3f)]
    private float contrast = 1f;

    [SerializeField, Range(0f, 1f)]
    private float noiseBlend = 0.35f;

    [Header("Layer 1")]
    [SerializeField]
    private Texture fillTexture;

    [SerializeField]
    private Vector2 tiling = Vector2.one;

    [SerializeField]
    private Vector2 scrollSpeed =
        new Vector2(0f, 0.08f);

    [Header("Layer 2")]
    [SerializeField]
    private Texture fillTexture2;

    [SerializeField]
    private Vector2 tiling2 =
        new Vector2(1.35f, 1.35f);

    [SerializeField]
    private Vector2 scrollSpeed2 =
        new Vector2(0f, -0.045f);

    [Header("Animation")]
    [SerializeField]
    private bool animate = true;

    private Image image;
    private Material runtimeMaterial;

    private static readonly int FillTexProperty =
        Shader.PropertyToID("_FillTex");

    private static readonly int FillTex2Property =
        Shader.PropertyToID("_FillTex2");

    private static readonly int ColorProperty =
        Shader.PropertyToID("_Color");

    private static readonly int BrightnessProperty =
        Shader.PropertyToID("_Brightness");

    private static readonly int ContrastProperty =
        Shader.PropertyToID("_Contrast");

    private static readonly int NoiseBlendProperty =
        Shader.PropertyToID("_NoiseBlend");

    private static readonly int TilingProperty =
        Shader.PropertyToID("_Tiling");

    private static readonly int ScrollSpeedProperty =
        Shader.PropertyToID("_ScrollSpeed");

    private static readonly int Tiling2Property =
        Shader.PropertyToID("_Tiling2");

    private static readonly int ScrollSpeed2Property =
        Shader.PropertyToID("_ScrollSpeed2");

    public bool Animate
    {
        get => animate;
        set
        {
            if (animate == value)
                return;

            animate = value;
            ApplyMaterialProperties();
        }
    }

    public Color Color
    {
        get => color;
        set
        {
            color = value;
            ApplyMaterialProperties();
        }
    }

    public float Brightness
    {
        get => brightness;
        set
        {
            brightness = Mathf.Clamp(
                value,
                0f,
                3f);

            ApplyMaterialProperties();
        }
    }

    public float Contrast
    {
        get => contrast;
        set
        {
            contrast = Mathf.Clamp(
                value,
                0f,
                3f);

            ApplyMaterialProperties();
        }
    }

    public float NoiseBlend
    {
        get => noiseBlend;
        set
        {
            noiseBlend = Mathf.Clamp01(value);
            ApplyMaterialProperties();
        }
    }

    public Texture FillTexture
    {
        get => fillTexture;
        set
        {
            fillTexture = value;
            ApplyMaterialProperties();
        }
    }

    public Vector2 Tiling
    {
        get => tiling;
        set
        {
            tiling = new Vector2(
                Mathf.Max(0.01f, value.x),
                Mathf.Max(0.01f, value.y));

            ApplyMaterialProperties();
        }
    }

    public Vector2 ScrollSpeed
    {
        get => scrollSpeed;
        set
        {
            scrollSpeed = value;
            ApplyMaterialProperties();
        }
    }

    public Texture FillTexture2
    {
        get => fillTexture2;
        set
        {
            fillTexture2 = value;
            ApplyMaterialProperties();
        }
    }

    public Vector2 Tiling2
    {
        get => tiling2;
        set
        {
            tiling2 = new Vector2(
                Mathf.Max(0.01f, value.x),
                Mathf.Max(0.01f, value.y));

            ApplyMaterialProperties();
        }
    }

    public Vector2 ScrollSpeed2
    {
        get => scrollSpeed2;
        set
        {
            scrollSpeed2 = value;
            ApplyMaterialProperties();
        }
    }

    public void RefreshMaterial()
    {
        ApplyMaterialProperties();
    }

    private void Awake()
    {
        image = GetComponent<Image>();

        CreateRuntimeMaterial();

        ApplyMaterialProperties();
    }

    private void CreateRuntimeMaterial()
    {
        if (image.material == null)
            return;

        runtimeMaterial =
            new Material(image.material);

        runtimeMaterial.name =
            image.material.name +
            " (Runtime)";

        image.material =
            runtimeMaterial;
    }

    private void ApplyMaterialProperties()
    {
        if (runtimeMaterial == null)
            return;

        runtimeMaterial.SetTexture(
            FillTexProperty,
            fillTexture);

        runtimeMaterial.SetTexture(
            FillTex2Property,
            fillTexture2);

        runtimeMaterial.SetColor(
            ColorProperty,
            color);

        runtimeMaterial.SetFloat(
            BrightnessProperty,
            brightness);

        runtimeMaterial.SetFloat(
            ContrastProperty,
            contrast);

        runtimeMaterial.SetFloat(
            NoiseBlendProperty,
            noiseBlend);

        runtimeMaterial.SetVector(
            TilingProperty,
            new Vector4(
                tiling.x,
                tiling.y,
                0f,
                0f));

        runtimeMaterial.SetVector(
            Tiling2Property,
            new Vector4(
                tiling2.x,
                tiling2.y,
                0f,
                0f));

        runtimeMaterial.SetVector(
            ScrollSpeedProperty,
            new Vector4(
                animate ? scrollSpeed.x : 0f,
                animate ? scrollSpeed.y : 0f,
                0f,
                0f));

        runtimeMaterial.SetVector(
            ScrollSpeed2Property,
            new Vector4(
                animate ? scrollSpeed2.x : 0f,
                animate ? scrollSpeed2.y : 0f,
                0f,
                0f));

        RefreshRenderingMaterial();
    }

    private void RefreshRenderingMaterial()
    {
        if (image == null)
            return;

        Material renderingMaterial =
            image.materialForRendering;

        if (renderingMaterial != null &&
            renderingMaterial != runtimeMaterial)
        {
            StencilMaterial.Remove(
                renderingMaterial);
        }

        image.SetMaterialDirty();
    }

    public void ResetAnimation()
    {
        // Animation is shader-driven.
        // No runtime animation state is required.
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
            runtimeMaterial = null;
        }
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        tiling.x =
            Mathf.Max(
                0.01f,
                tiling.x);

        tiling.y =
            Mathf.Max(
                0.01f,
                tiling.y);

        tiling2.x =
            Mathf.Max(
                0.01f,
                tiling2.x);

        tiling2.y =
            Mathf.Max(
                0.01f,
                tiling2.y);

        brightness =
            Mathf.Clamp(
                brightness,
                0f,
                3f);

        contrast =
            Mathf.Clamp(
                contrast,
                0f,
                3f);

        noiseBlend =
            Mathf.Clamp01(
                noiseBlend);

        if (!Application.isPlaying)
            return;

        if (image == null)
            image =
                GetComponent<Image>();

        ApplyMaterialProperties();
    }

#endif
}