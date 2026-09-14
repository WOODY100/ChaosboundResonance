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
            if (settingsRuntime == null ||
                settingsRuntime.Settings == null)
            {
                return;
            }

            GameSettings settings =
                settingsRuntime.Settings;

            if (confirmWorldItemDropToggle != null)
            {
                settings.ConfirmWorldItemDrop =
                    confirmWorldItemDropToggle.isOn;
            }

            if (showDamageNumbersToggle != null)
            {
                settings.ShowDamageNumbers =
                    showDamageNumbersToggle.isOn;
            }

            if (showEnemyHealthBarsToggle != null)
            {
                settings.ShowEnemyHealthBars =
                    showEnemyHealthBarsToggle.isOn;
            }

            if (vibrationToggle != null)
            {
                settings.Vibration =
                    vibrationToggle.isOn;
            }
        }

        public void RestoreDefaults()
        {
            if (confirmWorldItemDropToggle != null)
            {
                confirmWorldItemDropToggle.isOn = true;
            }

            if (showDamageNumbersToggle != null)
            {
                showDamageNumbersToggle.isOn = true;
            }

            if (showEnemyHealthBarsToggle != null)
            {
                showEnemyHealthBarsToggle.isOn = true;
            }

            if (vibrationToggle != null)
            {
                vibrationToggle.isOn = true;
            }
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
    }
}