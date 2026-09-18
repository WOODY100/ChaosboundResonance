using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class PersonalStashFilterUI
        : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private PersonalStashInventoryUI inventoryUI;

        [SerializeField]
        private TMP_Text filterLabel;

        [SerializeField]
        private PersonalStashFilterDropdownController dropdownController;

        [Header("Options")]
        [SerializeField]
        private Button allButton;

        [SerializeField]
        private Button newButton;

        [SerializeField]
        private Button weaponsButton;

        [SerializeField]
        private Button armorButton;

        [SerializeField]
        private Button accessoriesButton;

        [SerializeField]
        private Button relicsButton;

        private void Start()
        {
            ValidateConfiguration();
            RegisterListeners();

            ApplyFilter(
                PersonalStashInventoryFilter.All);
        }

        private void OnDestroy()
        {
            UnregisterListeners();
        }

        private void RegisterListeners()
        {
            if (allButton != null)
            {
                allButton.onClick.AddListener(
                    OnAllSelected);
            }

            if (newButton != null)
            {
                newButton.onClick.AddListener(
                    OnNewSelected);
            }

            if (weaponsButton != null)
            {
                weaponsButton.onClick.AddListener(
                    OnWeaponsSelected);
            }

            if (armorButton != null)
            {
                armorButton.onClick.AddListener(
                    OnArmorSelected);
            }

            if (accessoriesButton != null)
            {
                accessoriesButton.onClick.AddListener(
                    OnAccessoriesSelected);
            }

            if (relicsButton != null)
            {
                relicsButton.onClick.AddListener(
                    OnRelicsSelected);
            }
        }

        private void UnregisterListeners()
        {
            if (allButton != null)
            {
                allButton.onClick.RemoveListener(
                    OnAllSelected);
            }

            if (newButton != null)
            {
                newButton.onClick.RemoveListener(
                    OnNewSelected);
            }

            if (weaponsButton != null)
            {
                weaponsButton.onClick.RemoveListener(
                    OnWeaponsSelected);
            }

            if (armorButton != null)
            {
                armorButton.onClick.RemoveListener(
                    OnArmorSelected);
            }

            if (accessoriesButton != null)
            {
                accessoriesButton.onClick.RemoveListener(
                    OnAccessoriesSelected);
            }

            if (relicsButton != null)
            {
                relicsButton.onClick.RemoveListener(
                    OnRelicsSelected);
            }
        }

        private void OnAllSelected()
        {
            ApplyFilter(
                PersonalStashInventoryFilter.All);
        }

        private void OnNewSelected()
        {
            ApplyFilter(
                PersonalStashInventoryFilter.New);
        }

        private void OnWeaponsSelected()
        {
            ApplyFilter(
                PersonalStashInventoryFilter.Weapons);
        }

        private void OnArmorSelected()
        {
            ApplyFilter(
                PersonalStashInventoryFilter.Armor);
        }

        private void OnAccessoriesSelected()
        {
            ApplyFilter(
                PersonalStashInventoryFilter.Accessories);
        }

        private void OnRelicsSelected()
        {
            ApplyFilter(
                PersonalStashInventoryFilter.Relics);
        }

        private void ApplyFilter(
            PersonalStashInventoryFilter filter)
        {
            if (inventoryUI == null)
                return;

            inventoryUI.SetFilter(
                filter);

            UpdateLabel(filter);

            if (dropdownController != null)
            {
                dropdownController.CloseDropdown();
            }
        }

        private void UpdateLabel(
            PersonalStashInventoryFilter filter)
        {
            if (filterLabel == null)
                return;

            switch (filter)
            {
                case PersonalStashInventoryFilter.All:
                    filterLabel.text = "ALL";
                    break;

                case PersonalStashInventoryFilter.New:
                    filterLabel.text = "NEW";
                    break;

                case PersonalStashInventoryFilter.Weapons:
                    filterLabel.text = "WEAPONS";
                    break;

                case PersonalStashInventoryFilter.Armor:
                    filterLabel.text = "ARMOR";
                    break;

                case PersonalStashInventoryFilter.Accessories:
                    filterLabel.text = "ACCESSORIES";
                    break;

                case PersonalStashInventoryFilter.Relics:
                    filterLabel.text = "RELICS";
                    break;
            }
        }

        private void ValidateConfiguration()
        {
            if (inventoryUI == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersonalStashFilterUI] " +
                    "PersonalStashInventoryUI reference " +
                    "is missing.",
                    this);
            }

            if (filterLabel == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersonalStashFilterUI] " +
                    "Filter Label reference is missing.",
                    this);
            }
        }
    }
}