using System;
using Chaosbound.Gameplay.Save;
using UnityEngine;

namespace Chaosbound.Core.Settings
{
    public sealed class VideoSettingsPersistenceService
    {
        private readonly ISaveStorage storage;

        public VideoSettingsPersistenceService(ISaveStorage storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            this.storage = storage;
        }

        public VideoSettingsData LoadOrCreateDefaults()
        {
            if (!storage.TryLoad(out string json) ||
                string.IsNullOrWhiteSpace(json))
            {
                return CreateDefaults();
            }

            try
            {
                VideoSettingsData data =
                    JsonUtility.FromJson<VideoSettingsData>(json);

                if (data == null)
                {
                    Debug.LogWarning(
                        "[VideoSettingsPersistenceService] " +
                        "Saved video settings were invalid. " +
                        "Using defaults.");

                    return CreateDefaults();
                }

                return data;
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[VideoSettingsPersistenceService] " +
                    "Could not read saved video settings. " +
                    "Using defaults. " + exception.Message);

                return CreateDefaults();
            }
        }

        public void Save(VideoSettingsData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            string json = JsonUtility.ToJson(data, true);
            storage.Save(json);
        }

        private VideoSettingsData CreateDefaults()
        {
            VideoSettingsData defaults = new VideoSettingsData();
            Save(defaults);
            return defaults;
        }
    }
}