using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillEvolutionOptionUI :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Evolution")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Result")]
    [SerializeField] private Image resultIcon;
    [SerializeField] private TMP_Text resultNameText;

    [Header("Transfer")]
    [SerializeField] private GameObject retainedHeader;
    [SerializeField] private Transform retainedContainer;
    [SerializeField] private GameObject droppedHeader;
    [SerializeField] private Transform droppedContainer;

    [Header("Modifier")]
    [SerializeField] private SkillEvolutionModifierUI modifierPrefab;

    [Header("Interaction")]
    [SerializeField] private Button button;
    [SerializeField] private Image frame;

    [Header("Hover Effect")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float hoverSpeed = 8f;

    private Vector3 originalScale;
    private bool isHovered;
    private bool isInteractable = true;

    private Color baseFrameColor;

    private SkillEvolutionChoice currentChoice;
    private Action<SkillEvolutionChoice> onClick;

    private void Awake()
    {
        originalScale = transform.localScale;

        if (button == null)
            button = GetComponent<Button>();

        if (frame != null)
            baseFrameColor = frame.color;
    }

    private void Update()
    {
        Vector3 targetScale =
            isHovered
                ? originalScale * hoverScale
                : originalScale;

        transform.localScale =
            Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.unscaledDeltaTime * hoverSpeed);
    }

    public void Initialize(
        SkillEvolutionChoice choice,
        Action<SkillEvolutionChoice> callback)
    {
        ClearModifiers();

        currentChoice = choice;

        onClick = callback;

        originalScale = transform.localScale;

        ResetVisualState();

        if (choice == null)
            return;

        SkillEvolutionDefinition evolution =
            choice.Evolution;

        if (evolution == null)
            return;

        SetEvolutionVisuals(evolution);
        SetResultVisuals(evolution);
        SetTransferVisuals(choice);
    }

    private void SetEvolutionVisuals(
        SkillEvolutionDefinition evolution)
    {
        if (iconImage != null)
        {
            iconImage.sprite =
                evolution.Icon;

            iconImage.enabled =
                evolution.Icon != null;
        }

        if (titleText != null)
            titleText.text =
                evolution.DisplayName;

        if (rarityText != null)
        {
            rarityText.text =
                evolution.Rarity
                    .ToString()
                    .ToUpper();

            ApplyRarityVisual(
                evolution.Rarity);
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                evolution.Description;
        }
    }

    private void SetResultVisuals(
        SkillEvolutionDefinition evolution)
    {
        SkillDefinition result =
            evolution.ResultingSkill;

        if (result == null)
        {
            if (resultIcon != null)
                resultIcon.enabled = false;

            if (resultNameText != null)
                resultNameText.text =
                    string.Empty;

            return;
        }

        if (resultIcon != null)
        {
            resultIcon.sprite =
                result.Icon;

            resultIcon.enabled =
                result.Icon != null;
        }

        if (resultNameText != null)
        {
            resultNameText.text =
                result.DisplayName;
        }
    }

    private void SetTransferVisuals(
    SkillEvolutionChoice choice)
    {
        SkillEvolutionTransferPreview preview =
            choice.TransferPreview;

        if (preview == null)
        {
            SetSectionVisible(
                retainedHeader,
                retainedContainer,
                false);

            SetSectionVisible(
                droppedHeader,
                droppedContainer,
                false);

            return;
        }

        bool hasRetained =
            preview.RetainedModifiers != null &&
            preview.RetainedModifiers.Count > 0;

        bool hasDropped =
            preview.DroppedModifiers != null &&
            preview.DroppedModifiers.Count > 0;

        SetSectionVisible(
            retainedHeader,
            retainedContainer,
            hasRetained);

        SetSectionVisible(
            droppedHeader,
            droppedContainer,
            hasDropped);

        if (hasRetained)
        {
            CreateGroupedModifiers(
                retainedContainer,
                preview.RetainedModifiers,
                true);
        }

        if (hasDropped)
        {
            CreateGroupedModifiers(
                droppedContainer,
                preview.DroppedModifiers,
                false);
        }
    }

    private void CreateGroupedModifiers(
    Transform container,
    IReadOnlyList<SkillModifierDefinition> modifiers,
    bool retained)
    {
        if (container == null ||
            modifiers == null ||
            modifiers.Count == 0)
        {
            return;
        }

        Dictionary<SkillModifierDefinition, int> counts =
            new Dictionary<SkillModifierDefinition, int>();

        foreach (SkillModifierDefinition modifier in modifiers)
        {
            if (modifier == null)
                continue;

            if (counts.ContainsKey(modifier))
                counts[modifier]++;
            else
                counts.Add(modifier, 1);
        }

        foreach (KeyValuePair<SkillModifierDefinition, int> entry
                 in counts)
        {
            CreateModifier(
                container,
                entry.Key,
                retained,
                entry.Value);
        }
    }

    private void CreateModifier(
        Transform container,
        SkillModifierDefinition modifier,
        bool retained,
        int count)
    {
        if (container == null ||
            modifierPrefab == null ||
            modifier == null)
        {
            return;
        }

        SkillEvolutionModifierUI modifierUI =
            Instantiate(
                modifierPrefab,
                container);

        modifierUI.SetModifier(
            modifier,
            retained,
            count);
    }

    private void ClearModifiers()
    {
        ClearContainer(
            retainedContainer);

        ClearContainer(
            droppedContainer);
    }

    private void ClearContainer(
        Transform container)
    {
        if (container == null)
            return;

        for (int i = container.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(
                container.GetChild(i).gameObject);
        }
    }

    public void Clear()
    {
        ClearModifiers();

        currentChoice = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (titleText != null)
            titleText.text = string.Empty;

        if (rarityText != null)
            rarityText.text = string.Empty;

        if (descriptionText != null)
            descriptionText.text = string.Empty;

        if (resultIcon != null)
        {
            resultIcon.sprite = null;
            resultIcon.enabled = false;
        }

        if (resultNameText != null)
            resultNameText.text = string.Empty;

        SetSectionVisible(
            retainedHeader,
            retainedContainer,
            false);

        SetSectionVisible(
            droppedHeader,
            droppedContainer,
            false);
    }

    private void SetSectionVisible(
        GameObject header,
        Transform container,
        bool visible)
    {
        if (header != null)
            header.SetActive(visible);

        if (container != null)
            container.gameObject.SetActive(
                visible);
    }

    //==========================================================
    // Interaction
    //==========================================================

    public void SetInteractable(
        bool value)
    {
        isInteractable = value;

        if (button != null)
            button.interactable = value;

        if (!value)
        {
            isHovered = false;
            transform.localScale =
                originalScale;

            RestoreFrameColor();
        }
    }

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (!isInteractable)
            return;

        isHovered = true;

        if (frame != null)
            frame.color =
                baseFrameColor * 1.2f;
    }

    public void OnPointerExit(
        PointerEventData eventData)
    {
        if (!isInteractable)
            return;

        isHovered = false;

        RestoreFrameColor();
    }

    private void RestoreFrameColor()
    {
        if (frame != null)
            frame.color =
                baseFrameColor;
    }

    private void HandleClicked()
    {
        if (!isInteractable)
            return;

        onClick?.Invoke(
            currentChoice);
    }

    //==========================================================
    // Rarity
    //==========================================================

    private void ApplyRarityVisual(
        SkillRarity rarity)
    {
        Color color =
            GetRarityColor(rarity);

        baseFrameColor =
            color;

        if (frame != null)
            frame.color = color;

        if (rarityText != null)
            rarityText.color = color;
    }

    private Color GetRarityColor(
        SkillRarity rarity)
    {
        switch (rarity)
        {
            case SkillRarity.Common:
                return new Color(
                    0.7f,
                    0.7f,
                    0.7f);

            case SkillRarity.Rare:
                return new Color(
                    0.2f,
                    0.5f,
                    1f);

            case SkillRarity.Epic:
                return new Color(
                    0.6f,
                    0.2f,
                    1f);

            case SkillRarity.Legendary:
                return new Color(
                    1f,
                    0.75f,
                    0.2f);

            default:
                return Color.white;
        }
    }

    //==========================================================
    // Reset
    //==========================================================

    private void ResetVisualState()
    {
        isHovered = false;
        isInteractable = true;

        transform.localScale =
            originalScale;

        if (button != null)
        {
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(
                HandleClicked);
        }

        RestoreFrameColor();
    }
}