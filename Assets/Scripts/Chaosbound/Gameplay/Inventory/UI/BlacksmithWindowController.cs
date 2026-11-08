using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class BlacksmithWindowController : MonoBehaviour
    {
        [Header("Window")]
        [SerializeField] private Button closeButton;
        [SerializeField] private BlacksmithInventoryUI inventoryUI;

        public event Action OnWindowClosed;

        private Coroutine refreshRoutine;
        private bool isOpen;

        private void OnEnable()
        {
            isOpen = true;

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseClicked);
                closeButton.onClick.AddListener(HandleCloseClicked);
            }
        }

        private void OnDisable()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseClicked);
            }

            if (refreshRoutine != null)
            {
                StopCoroutine(refreshRoutine);
                refreshRoutine = null;
            }

            if (isOpen)
            {
                isOpen = false;
                OnWindowClosed?.Invoke();
            }
        }

        public void OpenWindow()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            if (refreshRoutine != null)
            {
                StopCoroutine(refreshRoutine);
                refreshRoutine = null;
            }

            refreshRoutine = StartCoroutine(
                RefreshInventoryNextFrame());
        }

        public void CloseWindow()
        {
            if (!gameObject.activeSelf)
                return;

            gameObject.SetActive(false);
        }

        private IEnumerator RefreshInventoryNextFrame()
        {
            yield return null;

            refreshRoutine = null;

            if (!isActiveAndEnabled)
                yield break;

            if (inventoryUI == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithWindowController] " +
                    "BlacksmithInventoryUI reference is missing.",
                    this);

                yield break;
            }

            inventoryUI.RefreshInventory();
        }

        private void HandleCloseClicked()
        {
            CloseWindow();
        }
    }
}