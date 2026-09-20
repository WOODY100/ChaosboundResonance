using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Save
{
    public static class SaveGameJsonSerializer
    {
        public static string Serialize(
            SaveGameData saveData)
        {
            if (saveData == null)
            {
                throw new ArgumentNullException(
                    nameof(saveData));
            }

            return JsonUtility.ToJson(
                saveData,
                true);
        }

        public static SaveGameData Deserialize(
            string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException(
                    "Save JSON cannot be null or empty.",
                    nameof(json));
            }

            SaveGameData saveData;

            try
            {
                saveData =
                    JsonUtility.FromJson<SaveGameData>(
                        json);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    "Failed to deserialize SaveGameData JSON.",
                    exception);
            }

            if (saveData == null)
            {
                throw new InvalidOperationException(
                    "Deserialized SaveGameData is null.");
            }

            return saveData;
        }
    }
}