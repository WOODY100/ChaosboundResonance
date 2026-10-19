using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GenericConfirmationController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text cancelButtonText;
    [SerializeField] private TMP_Text confirmButtonText;

    [Header("Buttons")]
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    private Action confirmAction;
    private Action cancelAction;
    private bool isOpen;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (cancelButton != null)
            cancelButton.onClick.AddListener(Cancel);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(Confirm);

        Hide();
    }

    private void OnDestroy()
    {
        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(Cancel);

        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(Confirm);
    }

    public void Show(
        string title,
        string message,
        string cancelLabel,
        string confirmLabel,
        Action onConfirm,
        Action onCancel = null)
    {
        if (isOpen)
            return;

        if (onConfirm == null)
        {
            Debug.LogError(
                "[GenericConfirmationController] Confirm action is null.",
                this);
            return;
        }

        if (titleText != null)
            titleText.text = title;

        if (messageText != null)
            messageText.text = message;

        if (cancelButtonText != null)
            cancelButtonText.text = cancelLabel;

        if (confirmButtonText != null)
            confirmButtonText.text = confirmLabel;

        confirmAction = onConfirm;
        cancelAction = onCancel;
        isOpen = true;

        if (panelRoot != null)
            panelRoot.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void SetMessage(string message)
    {
        if (!isOpen || messageText == null)
        {
            return;
        }

        messageText.text = message;
    }

    public void Confirm()
    {
        if (!isOpen)
            return;

        Action action = confirmAction;

        ClosePanel();

        action?.Invoke();
    }

    public void Cancel()
    {
        if (!isOpen)
            return;

        Action action = cancelAction;

        ClosePanel();

        action?.Invoke();
    }

    public void Hide()
    {
        ClosePanel();
    }

    private void ClosePanel()
    {
        isOpen = false;

        confirmAction = null;
        cancelAction = null;

        if (panelRoot != null)
        {
            if (panelRoot.activeSelf)
                panelRoot.SetActive(false);
        }
        else if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}