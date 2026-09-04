using UnityEngine;
using UnityEngine.UI;

public class SkillBarUI : MonoBehaviour
{
    [SerializeField] private SkillSlotUI[] slots;

    [Header("Evolution Buttons")]
    [SerializeField] private Button[] evolutionButtons;

    private PlayerSkillLoadout loadout;
    private LevelUpManager levelUpManager;
    private SkillEvolutionManager skillEvolutionManager;

    //==========================================================
    // Initialization
    //==========================================================

    public void Initialize(
        PlayerSkillLoadout playerLoadout,
        LevelUpManager lvlManager)
    {
        Bind(
            playerLoadout,
            lvlManager);
    }

    public void Bind(
        PlayerSkillLoadout playerLoadout,
        LevelUpManager lvlManager)
    {
        UnbindLoadout();
        UnbindLevelUpManager();

        loadout = playerLoadout;
        levelUpManager = lvlManager;

        if (loadout != null)
        {
            loadout.OnLoadoutChanged +=
                RefreshAll;
        }

        if (levelUpManager != null)
        {
            levelUpManager.OnReplaceRequested +=
                EnterReplaceMode;

            levelUpManager.OnReplaceFinished +=
                ExitReplaceMode;

            levelUpManager.OnReplaceCancelled +=
                ExitReplaceMode;
        }

        RefreshAll();
    }

    public void BindEvolutionManager(
        SkillEvolutionManager manager)
    {
        UnbindEvolutionManager();

        skillEvolutionManager = manager;

        if (skillEvolutionManager != null)
        {
            skillEvolutionManager.OnEvolutionAvailable +=
                HandleEvolutionAvailable;
        }

        RefreshEvolutionButtons();
    }

    //==========================================================
    // Cleanup
    //==========================================================

    private void OnDestroy()
    {
        UnbindLoadout();
        UnbindLevelUpManager();
        UnbindEvolutionManager();

        RemoveEvolutionButtonListeners();
    }

    private void UnbindLoadout()
    {
        if (loadout == null)
            return;

        loadout.OnLoadoutChanged -=
            RefreshAll;

        loadout = null;
    }

    private void UnbindLevelUpManager()
    {
        if (levelUpManager == null)
            return;

        levelUpManager.OnReplaceRequested -=
            EnterReplaceMode;

        levelUpManager.OnReplaceFinished -=
            ExitReplaceMode;

        levelUpManager.OnReplaceCancelled -=
            ExitReplaceMode;

        levelUpManager = null;
    }

    private void UnbindEvolutionManager()
    {
        if (skillEvolutionManager == null)
            return;

        skillEvolutionManager.OnEvolutionAvailable -=
            HandleEvolutionAvailable;

        skillEvolutionManager = null;
    }

    //==========================================================
    // Refresh
    //==========================================================

    public void RefreshAll()
    {
        if (loadout == null)
            return;

        RuntimeSkill[] skills =
            loadout.GetAllSkills();

        int slotCount =
            Mathf.Min(
                slots.Length,
                skills.Length);

        for (int i = 0;
             i < slotCount;
             i++)
        {
            slots[i].SetIndex(i);
            slots[i].SetSkill(skills[i]);
        }

        RefreshEvolutionButtons();
    }

    //==========================================================
    // Evolution Buttons
    //==========================================================

    private void InitializeEvolutionButtonListeners()
    {
        RemoveEvolutionButtonListeners();

        if (evolutionButtons == null)
            return;

        for (int i = 0;
             i < evolutionButtons.Length;
             i++)
        {
            int slotIndex = i;

            Button button =
                evolutionButtons[i];

            if (button == null)
                continue;

            button.onClick.AddListener(
                () => HandleEvolutionButtonClicked(
                    slotIndex));
        }
    }

    private void RemoveEvolutionButtonListeners()
    {
        if (evolutionButtons == null)
            return;

        for (int i = 0;
             i < evolutionButtons.Length;
             i++)
        {
            Button button =
                evolutionButtons[i];

            if (button == null)
                continue;

            button.onClick.RemoveAllListeners();
        }
    }

    private void HandleEvolutionAvailable(
        RuntimeSkill skill,
        int slotIndex)
    {
        RefreshEvolutionButtons();
    }

    private void HandleEvolutionButtonClicked(
        int slotIndex)
    {
        if (skillEvolutionManager == null)
            return;

        skillEvolutionManager.RequestOpenEvolution(
            slotIndex);

        RefreshEvolutionButtons();
    }

    private void RefreshEvolutionButtons()
    {
        if (evolutionButtons == null)
            return;

        for (int i = 0;
             i < evolutionButtons.Length;
             i++)
        {
            Button button =
                evolutionButtons[i];

            if (button == null)
                continue;

            bool shouldBeVisible =
                false;

            if (skillEvolutionManager != null &&
                loadout != null &&
                i < PlayerSkillLoadout.MaxSlots)
            {
                RuntimeSkill skill =
                    loadout.GetSkill(i);

                shouldBeVisible =
                    skill != null &&
                    skill.IsEvolutionPending;
            }

            button.gameObject.SetActive(
                shouldBeVisible);

            button.interactable =
                shouldBeVisible;
        }
    }

    //==========================================================
    // Replace Mode
    //==========================================================

    private void EnterReplaceMode(
        SkillDefinition newSkill)
    {
        foreach (SkillSlotUI slot in slots)
        {
            if (slot != null)
                slot.EnableReplaceMode();
        }
    }

    public void ExitReplaceMode()
    {
        foreach (SkillSlotUI slot in slots)
        {
            if (slot != null)
                slot.DisableReplaceMode();
        }
    }

    //==========================================================
    // Unity
    //==========================================================

    private void Awake()
    {
        InitializeEvolutionButtonListeners();
    }
}