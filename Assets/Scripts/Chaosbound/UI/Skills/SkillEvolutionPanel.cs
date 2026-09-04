using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillEvolutionPanel : MonoBehaviour
{
    [Header("Current Skill")]
    [SerializeField] private Image currentSkillIcon;
    [SerializeField] private TMP_Text currentSkillNameText;
    [SerializeField] private TMP_Text currentSkillLevelText;

    [Header("Evolution Options")]
    [SerializeField] private SkillEvolutionOptionUI[] options;

    [Header("Actions")]
    [SerializeField] private Button nowNoButton;

    private SkillEvolutionManager evolutionManager;

    private bool isInitialized;

    //==========================================================
    // Initialization
    //==========================================================

    public void Initialize(
        SkillEvolutionManager manager)
    {
        if (isInitialized)
            return;

        evolutionManager =
            manager
            ?? throw new ArgumentNullException(
                nameof(manager));

        evolutionManager.OnEvolutionOpened +=
            HandleEvolutionOpened;

        evolutionManager.OnEvolutionClosed +=
            HandleEvolutionClosed;

        evolutionManager.OnEvolutionApplied +=
            HandleEvolutionApplied;

        if (nowNoButton != null)
        {
            nowNoButton.onClick.RemoveAllListeners();

            nowNoButton.onClick.AddListener(
                HandleNowNoClicked);
        }

        isInitialized = true;

        Hide();
    }

    //==========================================================
    // Evolution Opened
    //==========================================================

    private void HandleEvolutionOpened(
        SkillEvolutionPresentationData data)
    {
        if (data == null)
            return;

        Show(data);
    }

    private void Show(
        SkillEvolutionPresentationData data)
    {
        SetCurrentSkill(
            data.CurrentSkill);

        IReadOnlyList<SkillEvolutionChoice> choices =
            data.Choices;

        int optionCount =
            options != null
                ? options.Length
                : 0;

        for (int i = 0;
             i < optionCount;
             i++)
        {
            SkillEvolutionOptionUI option =
                options[i];

            if (option == null)
                continue;

            if (i < choices.Count)
            {
                option.Initialize(
                    choices[i],
                    HandleEvolutionSelected);

                option.SetInteractable(true);
            }
            else
            {
                option.Clear();
                option.SetInteractable(false);
            }
        }

        gameObject.SetActive(true);
    }

    //==========================================================
    // Current Skill
    //==========================================================

    private void SetCurrentSkill(
        RuntimeSkill skill)
    {
        if (skill == null)
            return;

        SkillDefinition definition =
            skill.Definition;

        if (definition == null)
            return;

        if (currentSkillIcon != null)
        {
            currentSkillIcon.sprite =
                definition.Icon;

            currentSkillIcon.enabled =
                definition.Icon != null;
        }

        if (currentSkillNameText != null)
        {
            currentSkillNameText.text =
                definition.DisplayName;
        }

        if (currentSkillLevelText != null)
        {
            currentSkillLevelText.text =
                $"Lv. {skill.Level}";
        }
    }

    //==========================================================
    // Selection
    //==========================================================

    private void HandleEvolutionSelected(
        SkillEvolutionChoice choice)
    {
        if (choice == null)
            return;

        if (evolutionManager == null)
            return;

        evolutionManager.ApplyEvolution(
            choice.Evolution);
    }

    //==========================================================
    // Now No
    //==========================================================

    private void HandleNowNoClicked()
    {
        if (evolutionManager == null)
            return;

        evolutionManager.CancelEvolution();
    }

    //==========================================================
    // Evolution Closed
    //==========================================================

    private void HandleEvolutionClosed(
        RuntimeSkill skill,
        int slotIndex)
    {
        Hide();
    }

    private void HandleEvolutionApplied(
        RuntimeSkill skill,
        int slotIndex,
        SkillEvolutionDefinition evolution)
    {
        Hide();
    }

    //==========================================================
    // Visibility
    //==========================================================

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    //==========================================================
    // Cleanup
    //==========================================================

    private void OnDestroy()
    {
        if (evolutionManager != null)
        {
            evolutionManager.OnEvolutionOpened -=
                HandleEvolutionOpened;

            evolutionManager.OnEvolutionClosed -=
                HandleEvolutionClosed;

            evolutionManager.OnEvolutionApplied -=
                HandleEvolutionApplied;
        }

        if (nowNoButton != null)
            nowNoButton.onClick.RemoveAllListeners();

        evolutionManager = null;
        isInitialized = false;
    }
}