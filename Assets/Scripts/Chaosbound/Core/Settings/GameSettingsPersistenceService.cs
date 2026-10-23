using System;
using UnityEngine;
using Chaosbound.Gameplay.Save;

namespace Chaosbound.Core.Settings
{
    public sealed class GameSettingsPersistenceService
    {
        private readonly ISaveStorage storage;

        public GameSettingsPersistenceService(
            ISaveStorage storage)
        {
            this.storage = storage ??
                throw new ArgumentNullException(nameof(storage));
        }

        public GameSettings LoadOrCreateDefaults()
        {
            if (!storage.TryLoad(out string json))
            {
                GameSettings defaults = new GameSettings();
                Save(defaults);
                return defaults;
            }

            try
            {
                GameSettingsData data =
                    JsonUtility.FromJson<GameSettingsData>(json);

                if (data == null)
                    throw new InvalidOperationException(
                        "Settings JSON produced a null object.");

                return data.ToSettings();
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[GameSettingsPersistenceService] " +
                    "Failed to load settings. Defaults will be used. " +
                    exception);

                return new GameSettings();
            }
        }

        public void Save(GameSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            GameSettingsData data =
                GameSettingsData.FromSettings(settings);

            string json = JsonUtility.ToJson(data, true);

            storage.Save(json);
        }
    }
}