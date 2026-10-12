using Chaosbound.Core.Composition;
using Chaosbound.Core.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Core.Settings.UI
{
    public sealed class GameplaySettingsUI : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField] private Toggle confirmWorldItemDropToggle;
        [SerializeField] private Toggle showDamageNumbersToggle;
        [SerializeField] private Toggle showEnemyHealthBarsToggle;
        [SerializeField] private Toggle vibrationToggle;

        private GameSettingsRuntime settingsRuntime;

        private void OnEnable()
        {
            ResolveSettingsRuntime();
            RefreshFromSettings();
        }

        private void ResolveSettingsRuntime()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                settingsRuntime = null;
                return;
            }

            settingsRuntime =
                bootstrapContext.GameSettingsRuntime;
        }

        public void RefreshFromSettings()
        {
            if (settingsRuntime == null ||
                settingsRuntime.Settings == null)
            {
                return;
            }

            GameSettings settings =
                settingsRuntime.Settings;

            SetToggleWithoutNotify(
                confirmWorldItemDropToggle,
                settings.ConfirmWorldItemDrop);

            SetToggleWithoutNotify(
                showDamageNumbersToggle,
                settings.ShowDamageNumbers);

            SetToggleWithoutNotify(
                showEnemyHealthBarsToggle,
                settings.ShowEnemyHealthBars);

            SetToggleWithoutNotify(
                vibrationToggle,
                settings.Vibration);
        }

        public void Apply()
        {
            if (!EnsureSettingsRuntime())
            {
                Debug.LogError(
                    "[GameplaySettingsUI] Could not initialize GameSettingsRuntime.",
                    this);
                return;
            }

            SaveFeedbackController feedbackController =
                ResolveSaveFeedbackController();

            if (feedbackController == null)
            {
                Debug.LogError(
                    "[GameplaySettingsUI] SaveFeedbackController was not found.",
                    this);
                return;
            }

            GameSettings current = settingsRuntime.Settings;

            GameSettings newSettings = new GameSettings
            {
                ConfirmWorldItemDrop =
                    confirmWorldItemDropToggle != null
                        ? confirmWorldItemDropToggle.isOn
                        : current.ConfirmWorldItemDrop,

                ShowDamageNumbers =
                    showDamageNumbersToggle != null
                        ? showDamageNumbersToggle.isOn
                        : current.ShowDamageNumbers,

                ShowEnemyHealthBars =
                    showEnemyHealthBarsToggle != null
                        ? showEnemyHealthBarsToggle.isOn
                        : current.ShowEnemyHealthBars,

                Vibration =
                    vibrationToggle != null
                        ? vibrationToggle.isOn
                        : current.Vibration
            };

            feedbackController.RunSaveOperation(
                () => settingsRuntime.ApplySettings(newSettings));
        }

        public void RestoreDefaults()
        {
            if (!EnsureSettingsRuntime())
            {
                Debug.LogError(
                    "[GameplaySettingsUI] Could not initialize GameSettingsRuntime.",
                    this);
                return;
            }

            SaveFeedbackController feedbackController =
                ResolveSaveFeedbackController();

            if (feedbackController == null)
            {
                Debug.LogError(
                    "[GameplaySettingsUI] SaveFeedbackController was not found.",
                    this);
                return;
            }

            feedbackController.RunSaveOperation(
                () =>
                {
                    settingsRuntime.RestoreDefaults();
                    RefreshFromSettings();
                });
        }

        private void SetToggleWithoutNotify(
            Toggle toggle,
            bool value)
        {
            if (toggle == null)
            {
                return;
            }

            toggle.SetIsOnWithoutNotify(value);
        }

        private SaveFeedbackController ResolveSaveFeedbackController()
        {
            BootstrapContext context = BootstrapContext.Current;

            if (context == null)
                return null;

            return context.GetComponentInChildren<SaveFeedbackController>(true);
        }

        private bool EnsureSettingsRuntime()
        {
            if (settingsRuntime != null &&
                settingsRuntime.IsInitialized)
            {
                return true;
            }

            ResolveSettingsRuntime();

            return settingsRuntime != null &&
                   settingsRuntime.IsInitialized;
        }
    }
}