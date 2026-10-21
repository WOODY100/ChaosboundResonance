using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Chaosbound.Core.Composition;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Gameplay.Save;

public sealed class SaveSlotPanelController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject saveSlotPanel;

    [Header("Title")]
    [SerializeField] private TMP_Text titleText;

    [Header("Slot Cards")]
    [SerializeField]
    private SaveSlotCardUI[] slotCards =
        new SaveSlotCardUI[SaveSlotService.SlotCount];

    [Header("Navigation")]
    [SerializeField] private Button backButton;

    [Header("Delete Confirmation")]
    private GenericConfirmationController confirmationController;

    private int pendingDeleteSlot = -1;
    private bool isNewGameMode;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel(bool startNewGame)
    {
        isNewGameMode = startNewGame;

        if (saveSlotPanel == null)
        {
            Debug.LogError(
                "[SaveSlotPanelController] SaveSlotPanel is not assigned.",
                this);
            return;
        }

        saveSlotPanel.SetActive(true);

        if (titleText != null)
        {
            titleText.text = isNewGameMode
                ? "Nueva partida"
                : "Continuar partida";
        }

        RefreshSlots();
    }

    private bool TryGetConfirmationController()
    {
        BootstrapContext context = BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                "[SaveSlotPanelController] " +
                "BootstrapContext is unavailable.",
                this);

            return false;
        }

        confirmationController =
            context.GetComponentInChildren
                <GenericConfirmationController>(true);

        if (confirmationController == null)
        {
            Debug.LogError(
                "[SaveSlotPanelController] " +
                "GenericConfirmationController was not found in Bootstrap.",
                this);

            return false;
        }

        return true;
    }

    public void ClosePanel()
    {
        if (saveSlotPanel != null)
            saveSlotPanel.SetActive(false);
    }

    public void RefreshSlots()
    {
        BootstrapContext context = BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                "[SaveSlotPanelController] BootstrapContext is unavailable.",
                this);
            return;
        }

        SaveSlotInfo[] slotInfos = context.GetAllSaveSlotInfo();

        if (slotInfos == null ||
            slotInfos.Length != SaveSlotService.SlotCount)
        {
            Debug.LogError(
                "[SaveSlotPanelController] Invalid slot information.",
                this);
            return;
        }

        if (slotCards == null ||
            slotCards.Length != SaveSlotService.SlotCount)
        {
            Debug.LogError(
                "[SaveSlotPanelController] Assign exactly three slot cards.",
                this);
            return;
        }

        for (int i = 0; i < slotCards.Length; i++)
        {
            if (slotCards[i] == null)
            {
                Debug.LogError(
                    $"[SaveSlotPanelController] Slot card {i + 1} is missing.",
                    this);
                continue;
            }

            slotCards[i].Initialize(
                i,
                slotInfos[i],
                HandleSelectSlot,
                RequestDeleteSlot,
                isNewGameMode,
                !isNewGameMode);
        }
    }

    private void HandleSelectSlot(int slotIndex)
    {
        BootstrapContext context = BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                "[SaveSlotPanelController] BootstrapContext is unavailable.",
                this);
            return;
        }

        SaveSlotInfo[] slots = context.GetAllSaveSlotInfo();

        if (slots == null ||
            slotIndex < 0 ||
            slotIndex >= slots.Length)
        {
            Debug.LogWarning(
                "[SaveSlotPanelController] Invalid slot selection.",
                this);
            return;
        }

        bool success;

        if (slots[slotIndex].IsEmpty)
        {
            success = context.TryStartNewGame(slotIndex);
        }
        else if (slots[slotIndex].HasSave)
        {
            success = context.TryLoadGameFromSlot(slotIndex);
        }
        else
        {
            Debug.LogWarning(
                "[SaveSlotPanelController] This slot contains an invalid save.",
                this);
            return;
        }

        if (!success)
        {
            Debug.LogError(
                "[SaveSlotPanelController] Could not prepare the selected slot.",
                this);
            RefreshSlots();
            return;
        }

        if (context.SceneTransitionService == null)
        {
            Debug.LogError(
                "[SaveSlotPanelController] SceneTransitionService is unavailable.",
                this);
            return;
        }

        context.SceneTransitionService.LoadScene(GameScene.Sanctuary);
    }

    private void RequestDeleteSlot(int slotIndex)
    {
        BootstrapContext context = BootstrapContext.Current;

        if (context == null)
            return;

        SaveSlotInfo[] slots = context.GetAllSaveSlotInfo();

        if (slots == null ||
            slotIndex < 0 ||
            slotIndex >= slots.Length)
        {
            return;
        }

        SaveSlotInfo slot = slots[slotIndex];

        if (!slot.HasSave && !slot.IsInvalid)
            return;

        if (!TryGetConfirmationController())
            return;

        confirmationController.Show(
            "Eliminar partida",
            $"¿Seguro que quieres eliminar el guardado de la ranura " +
            $"{slot.SlotNumber}?\nEsta acción no se puede deshacer.",
            "Cancelar",
            "Eliminar",
            () => ConfirmDeleteSlot(slotIndex));
    }

    private void ConfirmDeleteSlot(int slotIndex)
    {
        BootstrapContext context = BootstrapContext.Current;

        if (context == null)
            return;

        bool deleted = context.TryDeleteSaveSlot(slotIndex);

        if (!deleted)
        {
            Debug.LogWarning(
                "[SaveSlotPanelController] " +
                "The save slot could not be deleted.",
                this);
            return;
        }

        RefreshSlots();
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(ClosePanel);
    }
}
