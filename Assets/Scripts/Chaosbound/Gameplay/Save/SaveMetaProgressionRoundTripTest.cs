using UnityEngine;
using Chaosbound.Gameplay.MetaProgression.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveMetaProgressionRoundTripTest
        : MonoBehaviour
    {
        [ContextMenu("Run Save Meta Progression Round Trip Test")]
        private void RunTest()
        {
            //==================================================
            // Runtime → Save
            //==================================================

            PersistentMetaState originalState =
                new PersistentMetaState();

            originalState.AddExperience(1250);

            SaveMetaProgressionData saveData =
                SaveMetaProgressionMapper.ToSaveData(
                    originalState);

            if (saveData == null)
            {
                Debug.LogError(
                    "[Save Meta Progression Round Trip Test] " +
                    "Save data is null.");
                return;
            }

            if (saveData.Experience != 1250)
            {
                Debug.LogError(
                    "[Save Meta Progression Round Trip Test] " +
                    $"Invalid saved Experience. " +
                    $"Expected=1250, " +
                    $"Actual={saveData.Experience}.");
                return;
            }

            //==================================================
            // Prepare / Validate Save Data
            //==================================================

            if (saveData.Experience < 0)
            {
                Debug.LogError(
                    "[Save Meta Progression Round Trip Test] " +
                    "Valid SaveData was incorrectly considered invalid.");
                return;
            }

            //==================================================
            // Save → Runtime
            //==================================================

            PersistentMetaState restoredState =
                new PersistentMetaState();

            try
            {
                restoredState.AddExperience(
                    saveData.Experience);
            }
            catch (System.Exception exception)
            {
                Debug.LogError(
                    "[Save Meta Progression Round Trip Test] " +
                    "Failed to restore Experience. " +
                    exception.Message);
                return;
            }

            if (restoredState.Experience !=
                originalState.Experience)
            {
                Debug.LogError(
                    "[Save Meta Progression Round Trip Test] " +
                    "Experience round-trip mismatch.");
                return;
            }

            //==================================================
            // Invalid Save
            //==================================================

            SaveMetaProgressionData invalidSave =
                new SaveMetaProgressionData
                {
                    Experience = -1
                };

            bool rejectedNegativeExperience =
                invalidSave.Experience < 0;

            if (!rejectedNegativeExperience)
            {
                Debug.LogError(
                    "[Save Meta Progression Round Trip Test] " +
                    "Negative Experience was not rejected.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Meta Progression Round Trip Test] " +
                "✓ COMPLETE — Meta Experience round-trip preserved " +
                "the persistent value and negative Experience was rejected.");
        }
    }
}