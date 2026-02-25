using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private LevelUpOptionUI optionPrefab;
    
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private GameObject cancelButton;

    private LevelUpManager levelUpManager;
    private bool isInReplaceMode;
    private bool selectionLocked;

    private List<LevelUpOptionUI> spawnedOptions = new List<LevelUpOptionUI>();

    void Start()
    {
        levelUpManager = FindFirstObjectByType<LevelUpManager>();

        if (levelUpManager == null)
        {
            Debug.LogError("LevelUpManager not found!");
            return;
        }

        levelUpManager.OnLevelUpOptionsGenerated += ShowOptions;
        levelUpManager.OnReplaceRequested += HandleReplaceRequested;
        levelUpManager.OnReplaceFinished += HandleReplaceFinished;
        levelUpManager.OnReplaceCancelled += HandleReplaceCancelled;
        levelUpManager.OnLevelUpFinished += HandleLevelUpFinished;

        cancelButton.GetComponent<Button>().onClick.AddListener(OnCancelPressed);
        panelRoot.SetActive(false);
        cancelButton.SetActive(false);
    }

    private void ShowOptions(List<UpgradeOption> options)
    {
        panelRoot.SetActive(true);
        SetNormalInstruction();

        foreach (Transform child in optionsContainer)
            Destroy(child.gameObject);
        
        spawnedOptions.Clear();

        foreach (var option in options)
        {
            var instance = Instantiate(optionPrefab, optionsContainer);
            instance.Initialize(option, OnOptionSelected);
            spawnedOptions.Add(instance);
        }
    }

    private void OnOptionSelected(UpgradeOption option)
    {
        if (isInReplaceMode || selectionLocked)
            return;

        selectionLocked = true;
        levelUpManager.SelectUpgrade(option);
    }

    public void SetNormalInstruction()
    {
        instructionText.text = "Selecciona una mejora";
        cancelButton.SetActive(false);
    }

    public void SetReplaceInstruction()
    {
        instructionText.text = "Selecciona una habilidad para reemplazar";
        cancelButton.SetActive(true);
    }

    private void HandleReplaceRequested(SkillDefinition skill)
    {
        isInReplaceMode = true;
        SetReplaceInstruction();

        SetOptionsInteractable(false);
    }

    private void HandleReplaceFinished()
    {
        isInReplaceMode = false;
        panelRoot.SetActive(false);
    }

    private void HandleReplaceCancelled()
    {
        isInReplaceMode = false;
        selectionLocked = false;
        SetNormalInstruction();
        SetOptionsInteractable(true);
    }

    private void HandleLevelUpFinished()
    {
        isInReplaceMode = false;
        selectionLocked = false;
        panelRoot.SetActive(false);
    }

    private void SetOptionsInteractable(bool value)
    {
        spawnedOptions.RemoveAll(o => o == null);

        foreach (var option in spawnedOptions)
        {
            if (option != null)
                option.SetInteractable(value);
        }
    }

    private void OnCancelPressed()
    {
        levelUpManager.CancelReplaceMode();
    }
}