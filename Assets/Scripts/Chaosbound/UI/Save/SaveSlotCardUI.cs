using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Chaosbound.Gameplay.Save;

public sealed class SaveSlotCardUI : MonoBehaviour
{
    [Header("Slot")]
    [SerializeField] private TMP_Text slotTitle;
    [SerializeField] private TMP_Text slotEmpty;
    [SerializeField] private Image slotImage;

    [Header("Buttons")]
    [SerializeField] private Button buttonSelect;
    [SerializeField] private Button buttonDelete;

    private int slotIndex;
    private Action<int> onSelect;
    private Action<int> onDelete;

    private bool allowNewGame;
    private bool allowContinue;


    public void Initialize(
        int index,
        SaveSlotInfo slotInfo,
        System.Action<int> selectCallback,
        System.Action<int> deleteCallback,
        bool canStartNewGame,
        bool canContinue)
    {
        slotIndex = index;
        onSelect = selectCallback;
        onDelete = deleteCallback;

        allowNewGame = canStartNewGame;
        allowContinue = canContinue;

        if (buttonSelect != null)
        {
            buttonSelect.onClick.RemoveListener(HandleSelect);
            buttonSelect.onClick.AddListener(HandleSelect);
        }

        if (buttonDelete != null)
        {
            buttonDelete.onClick.RemoveListener(HandleDelete);
            buttonDelete.onClick.AddListener(HandleDelete);
        }

        Refresh(slotInfo);
    }


    public void Refresh(SaveSlotInfo slotInfo)
    {
        if (slotInfo == null)
        {
            Debug.LogError(
                "[SaveSlotCardUI] SaveSlotInfo is null.",
                this);
            return;
        }

        if (slotImage != null)
        {
            slotImage.gameObject.SetActive(slotInfo.IsEmpty);
        }

        if (slotTitle != null)
        {
            slotTitle.text = $"RANURA {slotInfo.SlotNumber}";
        }

        if (slotEmpty != null)
        {
            switch (slotInfo.Status)
            {
                case SaveSlotStatus.Empty:
                    slotEmpty.text = "Ranura vacía";
                    break;

                case SaveSlotStatus.Available:
                    slotEmpty.text = allowNewGame
                        ? "Ranura ocupada"
                        : "Partida disponible";
                    break;

                case SaveSlotStatus.Invalid:
                    slotEmpty.text = allowNewGame
                        ? "Ranura no disponible"
                        : "Guardado inválido";
                    break;
            }
        }

        if (buttonSelect != null)
        {
            buttonSelect.interactable =
                (allowNewGame && slotInfo.IsEmpty) ||
                (allowContinue && slotInfo.HasSave);
        }

        if (buttonDelete != null)
        {
            buttonDelete.interactable =
                slotInfo.HasSave || slotInfo.IsInvalid;
        }
    }

    private void HandleSelect()
    {
        onSelect?.Invoke(slotIndex);
    }

    private void HandleDelete()
    {
        onDelete?.Invoke(slotIndex);
    }

    private void OnDestroy()
    {
        if (buttonSelect != null)
        {
            buttonSelect.onClick.RemoveListener(HandleSelect);
        }

        if (buttonDelete != null)
        {
            buttonDelete.onClick.RemoveListener(HandleDelete);
        }
    }
}