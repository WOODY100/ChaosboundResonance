using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillEvolutionModifierUI : MonoBehaviour
{
    [Header("Visual Elements")]
    [SerializeField] private Image statusIcon;
    [SerializeField] private Image modifierIcon;
    [SerializeField] private TMP_Text modifierText;

    [Header("Status Icons")]
    [SerializeField] private Sprite retainedStatusSprite;
    [SerializeField] private Sprite droppedStatusSprite;

    public void SetModifier(
    SkillModifierDefinition modifier,
    bool retained,
    int count)
    {
        if (modifier == null)
        {
            Clear();
            return;
        }

        if (statusIcon != null)
        {
            statusIcon.sprite = retained
                ? retainedStatusSprite
                : droppedStatusSprite;
        }

        if (modifierIcon != null)
        {
            modifierIcon.sprite = modifier.Icon;
            modifierIcon.enabled = modifier.Icon != null;
        }

        if (modifierText != null)
        {
            modifierText.text =
                count > 1
                    ? $"{modifier.DisplayName} ×{count}"
                    : modifier.DisplayName;
        }
    }

    public void Clear()
    {
        if (statusIcon != null)
            statusIcon.sprite = null;

        if (modifierIcon != null)
        {
            modifierIcon.sprite = null;
            modifierIcon.enabled = false;
        }

        if (modifierText != null)
            modifierText.text = string.Empty;
    }
}