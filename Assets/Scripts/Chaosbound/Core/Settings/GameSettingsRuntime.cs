using System;
using UnityEngine;

namespace Chaosbound.Core.Settings
{
    public sealed class GameSettingsRuntime : MonoBehaviour
    {
        private GameSettings settings;
        private GameSettingsPersistenceService persistenceService;

        public GameSettings Settings => settings;

        public bool IsInitialized => settings != null;

        public event Action<GameSettings> SettingsChanged;

        public void Initialize(
            GameSettingsPersistenceService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            persistenceService = service;
            settings = persistenceService.LoadOrCreateDefaults();

            SettingsChanged?.Invoke(settings);
        }

        public void ApplySettings(GameSettings newSettings)
        {
            if (newSettings == null)
                throw new ArgumentNullException(nameof(newSettings));

            if (persistenceService == null)
            {
                Debug.LogError(
                    "[GameSettingsRuntime] Persistence service is not initialized.",
                    this);
                return;
            }

            persistenceService.Save(newSettings);

            settings = newSettings;

            SettingsChanged?.Invoke(settings);
        }

        public void RestoreDefaults()
        {
            ApplySettings(new GameSettings());
        }
    }
}