using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class SkillTreeNodeView : MonoBehaviour
{
    [Header("State Visuals")]
    [SerializeField]
    private GameObject stateMist;

    [SerializeField]
    private GameObject stateOverlay;

    [SerializeField]
    private GameObject lockedIcon;

    [Header("State Colors")]
    [SerializeField]
    private Color lockedOverlayColor = Color.gray;

    [SerializeField]
    private Color availableOverlayColor = Color.white;

    [SerializeField]
    private Color purchasedOverlayColor = Color.clear;

    [Header("Mist Colors")]
    [SerializeField]
    private Color availableMistColor = Color.white;

    [SerializeField]
    private Color purchasedMistColor = Color.white;

    [Header("Content Visuals")]
    [SerializeField]
    private TMPro.TMP_Text textTitle;

    [SerializeField]
    private Image imageIcon;

    [SerializeField]
    private TMPro.TMP_Text costText;

    private SkillTreeNodeDefinition definition;
    private RectTransform rectTransform;
    private SkillTreeVisualState visualState;
    private Image stateOverlayImage;

    private Button nodeButton;

    public event Action<SkillTreeNodeView> Selected;

    public SkillTreeNodeDefinition Definition
    {
        get { return definition; }
    }

    public string NodeId
    {
        get
        {
            return definition != null
                ? definition.NodeId
                : string.Empty;
        }
    }

    public RectTransform RectTransform
    {
        get { return rectTransform; }
    }

    public SkillTreeVisualState VisualState
    {
        get { return visualState; }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeNodeView)}] " +
                $"'{name}' requires a RectTransform.",
                this);
        }

        stateOverlayImage =
            stateOverlay != null
                ? stateOverlay.GetComponent<Image>()
                : null;

        nodeButton =
            GetComponent<Button>();

        if (nodeButton == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeNodeView)}] " +
                $"'{name}' requires a Button component.",
                this);
        }
        else
        {
            nodeButton.onClick.AddListener(
                HandleButtonClicked);
        }
    }

    public void Initialize(
        SkillTreeNodeDefinition nodeDefinition)
    {
        if (nodeDefinition == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeNodeView)}] " +
                "Cannot initialize with a null definition.",
                this);

            definition = null;
            return;
        }

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        definition = nodeDefinition;

        ApplyContentVisuals();
        ApplyMistPhase();
        ApplyVisualState();
    }

    public Vector2 GetConnectionPoint(
        Vector2 direction)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
            return Vector2.zero;

        if (direction.sqrMagnitude <= 0.000001f)
            return Vector2.zero;

        Vector2 normalizedDirection =
            direction.normalized;

        Vector2 halfSize =
            rectTransform.rect.size * 0.5f;

        float x =
            Mathf.Abs(normalizedDirection.x) > 0.000001f
                ? halfSize.x /
                  Mathf.Abs(normalizedDirection.x)
                : float.PositiveInfinity;

        float y =
            Mathf.Abs(normalizedDirection.y) > 0.000001f
                ? halfSize.y /
                  Mathf.Abs(normalizedDirection.y)
                : float.PositiveInfinity;

        float distance =
            Mathf.Min(x, y);

        return normalizedDirection * distance;
    }

    public void SetVisualState(
        SkillTreeVisualState state)
    {
        visualState = state;

        ApplyVisualState();
    }

    private void ApplyVisualState()
    {
        switch (visualState)
        {
            case SkillTreeVisualState.Locked:

                SetActive(
                    stateMist,
                    false);

                SetActive(
                    stateOverlay,
                    true);

                SetActive(
                    lockedIcon,
                    true);

                SetOverlayColor(
                    lockedOverlayColor);

                break;

            case SkillTreeVisualState.Available:

                SetActive(
                    stateMist,
                    true);

                SetActive(
                    stateOverlay,
                    true);

                SetActive(
                    lockedIcon,
                    false);

                SetOverlayColor(
                    availableOverlayColor);

                SetMistColor(
                    availableMistColor);

                break;

            case SkillTreeVisualState.Purchased:

                SetActive(
                    stateMist,
                    true);

                SetActive(
                    stateOverlay,
                    false);

                SetActive(
                    lockedIcon,
                    false);

                SetMistColor(
                    purchasedMistColor);

                break;
        }
    }

    private void SetOverlayColor(
    Color color)
    {
        if (stateOverlayImage == null)
            return;

        stateOverlayImage.color = color;
    }

    private void SetMistColor(
    Color color)
    {
        SkillTreeMistInstance[] mistInstances =
            GetComponentsInChildren<SkillTreeMistInstance>(
                true);

        for (int i = 0; i < mistInstances.Length; i++)
        {
            mistInstances[i].SetColor(color);
        }
    }

    private void SetActive(
        GameObject target,
        bool active)
    {
        if (target == null)
            return;

        target.SetActive(active);
    }

    private void ApplyMistPhase()
    {
        if (string.IsNullOrEmpty(NodeId))
            return;

        float phase =
            CalculateMistPhase(NodeId);

        SkillTreeMistInstance[] mistInstances =
            GetComponentsInChildren<SkillTreeMistInstance>(
                true);

        for (int i = 0; i < mistInstances.Length; i++)
        {
            mistInstances[i].SetPhase(phase);
        }
    }

    private float CalculateMistPhase(
        string value)
    {
        uint hash = 2166136261u;

        for (int i = 0; i < value.Length; i++)
        {
            hash ^= value[i];
            hash *= 16777619u;
        }

        // Avalanche mixing.
        hash ^= hash >> 16;
        hash *= 0x85EBCA6Bu;

        hash ^= hash >> 13;
        hash *= 0xC2B2AE35u;

        hash ^= hash >> 16;

        float normalized =
            hash / (float)uint.MaxValue;

        return normalized * Mathf.PI * 2f;
    }

    private void ApplyContentVisuals()
    {
        if (definition == null)
            return;

        if (textTitle != null)
        {
            textTitle.text =
                definition.DisplayName;
        }

        if (imageIcon != null)
        {
            imageIcon.sprite =
                definition.Icon;
        }

        if (costText != null)
        {
            costText.text =
                definition.Cost.ToString();
        }
    }

    private void HandleButtonClicked()
    {
        if (definition == null)
            return;

        Selected?.Invoke(this);
    }

    private void OnDestroy()
    {
        if (nodeButton != null)
        {
            nodeButton.onClick.RemoveListener(
                HandleButtonClicked);
        }
    }
}