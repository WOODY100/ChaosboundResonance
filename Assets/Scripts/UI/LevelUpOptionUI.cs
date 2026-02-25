using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpOptionUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Text Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;

    [Header("Visual Elements")]
    [SerializeField] private Image frameOverlay;
    [SerializeField] private Image bottomOrnament;
    [SerializeField] private TMP_Text rarityText;

    [Header("Hover Effect")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float hoverSpeed = 8f;

    private Vector3 originalScale;
    private bool isHovered;
    private Color baseFrameColor;

    private bool isInteractable = true;

    private UpgradeOption currentOption;
    private Action<UpgradeOption> onClick;

    private void Update()
    {
        Vector3 targetScale = isHovered
            ? originalScale * hoverScale
            : originalScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.unscaledDeltaTime * hoverSpeed);
    }

    private void Awake()
    {
        originalScale = transform.localScale;
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;

        isHovered = true;
        frameOverlay.color = baseFrameColor * 1.2f;
        bottomOrnament.color = baseFrameColor * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable) return;

        isHovered = false;
        frameOverlay.color = baseFrameColor;
        bottomOrnament.color = baseFrameColor;
    }

    public void SetInteractable(bool value)
    {
        isInteractable = value;

        if (button != null)
            button.interactable = value;

        if (!value)
        {
            isHovered = false;
            transform.localScale = originalScale;

            frameOverlay.color = baseFrameColor;
            bottomOrnament.color = baseFrameColor;
        }
    }

    public void Initialize(UpgradeOption option,
                       Action<UpgradeOption> callback)
    {
        currentOption = option;
        onClick = callback;

        titleText.text = GetTitle(option);
        descriptionText.text = GetDescription(option);

        iconImage.sprite = GetIcon(option);

        SkillRarity rarity = GetRarity(option);
        rarityText.text = rarity.ToString().ToUpper();

        ApplyRarityVisual(rarity);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(currentOption));
    }

    private Sprite GetIcon(UpgradeOption option)
    {
        switch (option.Type)
        {
            case UpgradeType.NewSkill:
                return option.SkillDefinition.Icon;

            case UpgradeType.SkillModifier:
                return option.ModifierDefinition.Icon;

            default:
                return null;
        }
    }

    private string GetTitle(UpgradeOption option)
    {
        switch (option.Type)
        {
            case UpgradeType.NewSkill:
                return option.SkillDefinition.DisplayName;

            case UpgradeType.SkillModifier:
                return option.ModifierDefinition.DisplayName;

            default:
                return "Upgrade";
        }
    }

    private string GetDescription(UpgradeOption option)
    {
        switch (option.Type)
        {
            case UpgradeType.NewSkill:
                return option.SkillDefinition.Description;

            case UpgradeType.SkillModifier:
                return option.ModifierDefinition.Description;

            default:
                return "";
        }
    }

    private SkillRarity GetRarity(UpgradeOption option)
    {
        switch (option.Type)
        {
            case UpgradeType.NewSkill:
                return option.SkillDefinition.Rarity;

            case UpgradeType.SkillModifier:
                return option.ModifierDefinition.Rarity;

            default:
                return SkillRarity.Common;
        }
    }

    private void ApplyRarityVisual(SkillRarity rarity)
    {
        Color color = GetRarityColor(rarity);

        baseFrameColor = color;

        frameOverlay.color = color;
        bottomOrnament.color = color;
        rarityText.color = color;
    }

    private Color GetRarityColor(SkillRarity rarity)
    {
        switch (rarity)
        {
            case SkillRarity.Common:
                return new Color(0.7f, 0.7f, 0.7f);

            case SkillRarity.Rare:
                return new Color(0.2f, 0.5f, 1f);

            case SkillRarity.Epic:
                return new Color(0.6f, 0.2f, 1f);

            case SkillRarity.Legendary:
                return new Color(1f, 0.75f, 0.2f);

            default:
                return Color.white;
        }
    }
}