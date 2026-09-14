using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class InventoryTabUI :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [Header("Button")]
        [SerializeField] private Button button;

        [Header("Active State")]
        [SerializeField] private GameObject activeRoot;
        [SerializeField] private GameObject activeNormal;
        [SerializeField] private GameObject activeHover;
        [SerializeField] private GameObject activePressed;

        [Header("Inactive State")]
        [SerializeField] private GameObject inactiveRoot;
        [SerializeField] private GameObject inactiveNormal;
        [SerializeField] private GameObject inactiveHover;
        [SerializeField] private GameObject inactivePressed;

        private bool isSelected;
        private bool isPointerOver;
        private bool isPressed;

        public Button Button => button;
        public bool IsSelected => isSelected;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            SetAllVisualObjectsInactive();
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            isPressed = false;

            RefreshVisualState();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button != null && !button.interactable)
                return;

            isPointerOver = true;
            RefreshVisualState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerOver = false;
            isPressed = false;

            RefreshVisualState();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (button != null && !button.interactable)
                return;

            isPressed = true;
            RefreshVisualState();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            RefreshVisualState();
        }

        private void RefreshVisualState()
        {
            SetAllVisualObjectsInactive();

            if (isSelected)
            {
                ShowState(
                    activeRoot,
                    activeNormal,
                    activeHover,
                    activePressed);
            }
            else
            {
                ShowState(
                    inactiveRoot,
                    inactiveNormal,
                    inactiveHover,
                    inactivePressed);
            }
        }

        private void ShowState(
            GameObject root,
            GameObject normal,
            GameObject hover,
            GameObject pressed)
        {
            if (root != null)
                root.SetActive(true);

            if (isPressed && pressed != null)
            {
                pressed.SetActive(true);
                return;
            }

            if (isPointerOver && hover != null)
            {
                hover.SetActive(true);
                return;
            }

            if (normal != null)
                normal.SetActive(true);
        }

        private void SetAllVisualObjectsInactive()
        {
            if (activeRoot != null)
                activeRoot.SetActive(false);

            if (inactiveRoot != null)
                inactiveRoot.SetActive(false);

            if (activeNormal != null)
                activeNormal.SetActive(false);

            if (activeHover != null)
                activeHover.SetActive(false);

            if (activePressed != null)
                activePressed.SetActive(false);

            if (inactiveNormal != null)
                inactiveNormal.SetActive(false);

            if (inactiveHover != null)
                inactiveHover.SetActive(false);

            if (inactivePressed != null)
                inactivePressed.SetActive(false);
        }
    }
}