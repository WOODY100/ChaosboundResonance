using UnityEngine;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameDataCompositionTest
        : MonoBehaviour
    {
        [ContextMenu("Run Save Game Data Composition Test")]
        private void RunTest()
        {
            SaveGameData saveData =
                new SaveGameData();

            saveData.SaveVersion = 1;

            //==================================================
            // Inventory
            //==================================================

            saveData.Inventory =
                new SaveInventoryData();

            if (saveData.Inventory == null)
            {
                Debug.LogError(
                    "[Save Game Data Composition Test] " +
                    "Inventory data could not be created.");
                return;
            }

            //==================================================
            // Equipment
            //==================================================

            saveData.Equipment =
                new SaveEquipmentData();

            if (saveData.Equipment == null)
            {
                Debug.LogError(
                    "[Save Game Data Composition Test] " +
                    "Equipment data could not be created.");
                return;
            }

            //==================================================
            // Meta Progression
            //==================================================

            saveData.MetaProgression =
                new SaveMetaProgressionData
                {
                    Experience = 1250
                };

            if (saveData.MetaProgression == null)
            {
                Debug.LogError(
                    "[Save Game Data Composition Test] " +
                    "Meta Progression data could not be created.");
                return;
            }

            //==================================================
            // Validate Save Version
            //==================================================

            if (saveData.SaveVersion != 1)
            {
                Debug.LogError(
                    "[Save Game Data Composition Test] " +
                    $"Invalid SaveVersion. " +
                    $"Expected=1, " +
                    $"Actual={saveData.SaveVersion}.");
                return;
            }

            //==================================================
            // Validate Meta
            //==================================================

            if (saveData.MetaProgression.Experience != 1250)
            {
                Debug.LogError(
                    "[Save Game Data Composition Test] " +
                    "Meta Progression Experience mismatch.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Game Data Composition Test] " +
                "✓ COMPLETE — SaveGameData successfully composed " +
                "Inventory, Equipment, and Meta Progression data.");
        }
    }
}