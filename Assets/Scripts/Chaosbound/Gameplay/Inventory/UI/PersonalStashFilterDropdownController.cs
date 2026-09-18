using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class PersonalStashFilterDropdownController
        : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Button dropdownButton;

        [SerializeField]
        private GameObject optionsRoot;

        [SerializeField]
        private RectTransform filterArrow;

        [Header("Animation")]
        [SerializeField]
        private float arrowOpenRotation = 180f;

        private bool isOpen;

        private void Awake()
        {
            SetOpen(false);
        }

        private void OnEnable()
        {
            if (dropdownButton != null)
            {
                dropdownButton.onClick.AddListener(
                    ToggleDropdown);
            }
        }

        private void OnDisable()
        {
            if (dropdownButton != null)
            {
                dropdownButton.onClick.RemoveListener(
                    ToggleDropdown);
            }
        }

        private void ToggleDropdown()
        {
            SetOpen(!isOpen);
        }

        public void CloseDropdown()
        {
            SetOpen(false);
        }

        private void SetOpen(bool open)
        {
            isOpen = open;

            if (optionsRoot != null)
            {
                optionsRoot.SetActive(isOpen);
            }

            UpdateArrow();
        }

        private void UpdateArrow()
        {
            if (filterArrow == null)
                return;

            float zRotation =
                isOpen
                    ? arrowOpenRotation
                    : 0f;

            filterArrow.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    zRotation);
        }

        private void ValidateConfiguration()
        {
            if (dropdownButton == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersonalStashFilterDropdownController] " +
                    "Dropdown Button reference is missing.",
                    this);
            }

            if (optionsRoot == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersonalStashFilterDropdownController] " +
                    "Options Root reference is missing.",
                    this);
            }

            if (filterArrow == null)
            {
                UnityEngine.Debug.LogWarning(
                    "[PersonalStashFilterDropdownController] " +
                    "Filter Arrow reference is missing.",
                    this);
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            ValidateConfiguration();
        }

#endif
    }
}