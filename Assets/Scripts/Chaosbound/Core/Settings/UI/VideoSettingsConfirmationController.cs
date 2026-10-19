using System;
using Chaosbound.Core.Composition;
using UnityEngine;

namespace Chaosbound.Core.Settings.UI
{
    public sealed class VideoSettingsConfirmationController : MonoBehaviour
    {
        private const float ConfirmationDuration = 15f;

        private GenericConfirmationController confirmationController;
        private VideoSettingsRuntime runtime;
        private Action onConfirmed;
        private Action onFinished;

        private float remainingTime;
        private bool isWaiting;

        public bool IsWaiting => isWaiting;

        public bool BeginPreview(
            VideoSettingsRuntime settingsRuntime,
            Action confirmAction,
            Action finishedAction = null)
        {
            if (settingsRuntime == null)
            {
                Debug.LogError(
                    "[VideoSettingsConfirmationController] Runtime is null.",
                    this);
                return false;
            }

            if (isWaiting)
            {
                Debug.LogWarning(
                    "[VideoSettingsConfirmationController] " +
                    "A confirmation is already active.",
                    this);
                return false;
            }

            ResolveConfirmationController();

            if (confirmationController == null)
            {
                Debug.LogError(
                    "[VideoSettingsConfirmationController] " +
                    "GenericConfirmationController was not found.",
                    this);
                return false;
            }

            if (!settingsRuntime.HasPendingPreview)
            {
                Debug.LogError(
                    "[VideoSettingsConfirmationController] " +
                    "There is no pending video settings preview.",
                    this);
                return false;
            }

            runtime = settingsRuntime;
            onConfirmed = confirmAction;
            onFinished = finishedAction;

            remainingTime = ConfirmationDuration;
            isWaiting = true;

            confirmationController.Show(
                "Confirmar ajustes gráficos",
                BuildMessage(),
                "REVERTIR",
                "ACEPTAR",
                Confirm,
                Revert);

            return true;
        }

        private void Update()
        {
            if (!isWaiting)
            {
                return;
            }

            remainingTime -= Time.unscaledDeltaTime;

            if (remainingTime <= 0f)
            {
                Revert();
                return;
            }

            if (confirmationController != null)
            {
                confirmationController.SetMessage(BuildMessage());
            }
        }

        private string BuildMessage()
        {
            int seconds = Mathf.CeilToInt(
                Mathf.Max(0f, remainingTime));

            return
                "¿Deseas conservar estos ajustes gráficos?\n\n" +
                "Los cambios se revertirán en " +
                seconds +
                " segundos si no los confirmas.";
        }

        private void Confirm()
        {
            if (!isWaiting)
            {
                return;
            }

            if (runtime == null || !runtime.ConfirmPreviewSettings())
            {
                Revert();
                return;
            }

            Action confirmed = onConfirmed;

            Finish();

            confirmed?.Invoke();
        }

        private void Revert()
        {
            if (!isWaiting)
            {
                return;
            }

            // Cerrar el panel también cuando la reversión viene del timeout.
            if (confirmationController != null)
            {
                confirmationController.Hide();
            }

            if (runtime != null)
            {
                runtime.RevertPreviewSettings();
            }

            Action finished = onFinished;

            Finish();

            finished?.Invoke();
        }

        private void Finish()
        {
            isWaiting = false;

            runtime = null;
            onConfirmed = null;
            onFinished = null;
            remainingTime = 0f;
        }

        private void ResolveConfirmationController()
        {
            BootstrapContext context = BootstrapContext.Current;

            confirmationController = context != null
                ? context.GetComponentInChildren
                    <GenericConfirmationController>(true)
                : null;
        }
    }
}