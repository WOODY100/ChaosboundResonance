using Chaosbound.SkillTree;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class SkillTreeNodeDetailsView : MonoBehaviour
{
    [Header("Content")]

    [SerializeField]
    private Image imageIcon;

    [SerializeField]
    private TMP_Text textTitle;

    [SerializeField]
    private TMP_Text textNodeType;

    [SerializeField]
    private TMP_Text textCost;

    [SerializeField]
    private TMP_Text textDescription;

    [SerializeField]
    private SkillTreeNodeRequirementDetailsView requirementDetailsView;

    [Header("Effects")]

    [SerializeField]
    private SkillTreeNodeEffectDetailsView effectDetailsView;

    [Header("Purchase Buttons")]

    [SerializeField]
    private GameObject lockedButton;

    [SerializeField]
    private GameObject availableButton;

    [SerializeField]
    private GameObject purchasedButton;

    [Header("Choice Buttons")]

    [SerializeField]
    private GameObject availableOptionButton01;

    [SerializeField]
    private GameObject purchasedOptionButton01;

    [SerializeField]
    private GameObject lockedOptionButton01;

    [SerializeField]
    private GameObject availableOptionButton02;

    [SerializeField]
    private GameObject purchasedOptionButton02;

    [SerializeField]
    private GameObject lockedOptionButton02;

    private SkillTreeVisualState visualState;

    private string selectedChoiceOptionId;

    private SkillTreeNodeDefinition definition;

    public event Action PurchaseRequested;

    public event Action<string> ChoiceRequested;

    public SkillTreeNodeDefinition Definition
    {
        get { return definition; }
    }

    public SkillTreeVisualState VisualState
    {
        get { return visualState; }
    }

    private void Awake()
    {
        ConfigureButtons();
    }

    public void Show(
        SkillTreeNodeDefinition nodeDefinition)
    {
        if (nodeDefinition == null)
        {
            Hide();
            return;
        }

        definition = nodeDefinition;

        visualState =
            SkillTreeVisualState.Locked;

        selectedChoiceOptionId = null;

        if (requirementDetailsView != null)
        {
            requirementDetailsView.Show(
                definition.Requirement);
        }

        if (effectDetailsView != null)
        {
            effectDetailsView.Show(
                definition);
        }

        gameObject.SetActive(true);

        ApplyContent();
    }

    public void Hide()
    {
        if (effectDetailsView != null)
        {
            effectDetailsView.Hide();
        }

        definition = null;

        visualState =
            SkillTreeVisualState.Locked;

        selectedChoiceOptionId = null;

        HidePurchaseButtons();

        gameObject.SetActive(false);
    }

    private void ApplyContent()
    {
        if (definition == null)
            return;

        if (imageIcon != null)
        {
            imageIcon.sprite =
                definition.Icon;
        }

        if (textTitle != null)
        {
            textTitle.text =
                definition.DisplayName;
        }

        if (textNodeType != null)
        {
            textNodeType.text =
                definition.NodeType.ToString();
        }

        if (textCost != null)
        {
            textCost.text =
                definition.Cost.ToString();
        }

        if (textDescription != null)
        {
            textDescription.text =
                definition.Description;
        }
    }

    public void SetRuntimeState(
        SkillTreeVisualState state,
        string selectedChoiceOptionId)
    {
        visualState = state;

        this.selectedChoiceOptionId =
            selectedChoiceOptionId;

        ApplyPurchaseState();
    }

    private void ApplyPurchaseState()
    {
        HidePurchaseButtons();

        if (definition == null)
            return;

        if (definition.NodeType ==
            SkillTreeNodeType.Choice)
        {
            ApplyChoiceButtons();
            return;
        }

        if (visualState ==
            SkillTreeVisualState.Locked)
        {
            ShowButton(lockedButton);
            return;
        }

        if (visualState ==
            SkillTreeVisualState.Available)
        {
            ShowButton(availableButton);
            return;
        }

        if (visualState ==
            SkillTreeVisualState.Purchased)
        {
            ShowButton(purchasedButton);
        }
    }

    private void ApplyChoiceButtons()
    {
        if (definition.ChoiceOptions == null)
            return;

        for (int i = 0;
             i < definition.ChoiceOptions.Length;
             i++)
        {
            if (definition.ChoiceOptions[i] == null)
                continue;

            string optionId =
                definition.ChoiceOptions[i].OptionId;

            SkillTreeVisualState optionState;

            if (visualState ==
                SkillTreeVisualState.Locked)
            {
                optionState =
                    SkillTreeVisualState.Locked;
            }
            else if (visualState ==
                     SkillTreeVisualState.Purchased)
            {
                if (optionId ==
                    selectedChoiceOptionId)
                {
                    optionState =
                        SkillTreeVisualState.Purchased;
                }
                else
                {
                    optionState =
                        SkillTreeVisualState.Locked;
                }
            }
            else
            {
                optionState =
                    SkillTreeVisualState.Available;
            }

            ApplyOptionButton(
                i,
                optionState);
        }
    }

    private void ApplyOptionButton(
    int optionIndex,
    SkillTreeVisualState state)
    {
        if (optionIndex == 0)
        {
            if (state ==
                SkillTreeVisualState.Locked)
            {
                ShowButton(lockedOptionButton01);
            }
            else if (state ==
                     SkillTreeVisualState.Available)
            {
                ShowButton(availableOptionButton01);
            }
            else
            {
                ShowButton(purchasedOptionButton01);
            }

            return;
        }

        if (optionIndex == 1)
        {
            if (state ==
                SkillTreeVisualState.Locked)
            {
                ShowButton(lockedOptionButton02);
            }
            else if (state ==
                     SkillTreeVisualState.Available)
            {
                ShowButton(availableOptionButton02);
            }
            else
            {
                ShowButton(purchasedOptionButton02);
            }
        }
    }

    private void HidePurchaseButtons()
    {
        SetButtonActive(
            lockedButton,
            false);

        SetButtonActive(
            availableButton,
            false);

        SetButtonActive(
            purchasedButton,
            false);

        SetButtonActive(
            availableOptionButton01,
            false);

        SetButtonActive(
            purchasedOptionButton01,
            false);

        SetButtonActive(
            lockedOptionButton01,
            false);

        SetButtonActive(
            availableOptionButton02,
            false);

        SetButtonActive(
            purchasedOptionButton02,
            false);

        SetButtonActive(
            lockedOptionButton02,
            false);
    }

    private void ShowButton(
        GameObject button)
    {
        SetButtonActive(
            button,
            true);
    }

    private void SetButtonActive(
        GameObject button,
        bool active)
    {
        if (button == null)
            return;

        button.SetActive(active);
    }

    private void ConfigureButtons()
    {
        ConfigurePurchaseButton(
            availableButton);

        ConfigureChoiceButton(
            availableOptionButton01,
            0);

        ConfigureChoiceButton(
            availableOptionButton02,
            1);
    }

    private void ConfigurePurchaseButton(
    GameObject buttonObject)
    {
        if (buttonObject == null)
            return;

        Button button =
            buttonObject.GetComponent<Button>();

        if (button == null)
            return;

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(
            HandlePurchaseClicked);
    }

    private void ConfigureChoiceButton(
    GameObject buttonObject,
    int optionIndex)
    {
        if (buttonObject == null)
            return;

        Button button =
            buttonObject.GetComponent<Button>();

        if (button == null)
            return;

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(
            delegate
            {
                HandleChoiceClicked(optionIndex);
            });
    }

    private void HandlePurchaseClicked()
    {
        if (definition == null)
            return;

        PurchaseRequested?.Invoke();
    }

    private void HandleChoiceClicked(
    int optionIndex)
    {
        if (definition == null)
            return;

        if (definition.ChoiceOptions == null)
            return;

        if (optionIndex < 0 ||
            optionIndex >= definition.ChoiceOptions.Length)
        {
            return;
        }

        if (definition.ChoiceOptions[optionIndex] == null)
            return;

        string optionId =
            definition.ChoiceOptions[optionIndex].OptionId;

        if (string.IsNullOrWhiteSpace(optionId))
            return;

        ChoiceRequested?.Invoke(optionId);
    }
}