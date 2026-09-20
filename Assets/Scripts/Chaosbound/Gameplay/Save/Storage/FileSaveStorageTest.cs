using UnityEngine;

namespace Chaosbound.Gameplay.Save
{
    public sealed class FileSaveStorageTest
        : MonoBehaviour
    {
        private const string TestFileName =
            "chaosbound_storage_test.json";

        [ContextMenu("Run File Save Storage Test")]
        private void RunTest()
        {
            FileSaveStorage storage =
                new FileSaveStorage(
                    TestFileName);

            const string originalData =
                "{\"test\":\"chaosbound\",\"value\":12345}";

            //==================================================
            // Clean Start
            //==================================================

            try
            {
                storage.Delete();
            }
            catch (System.Exception exception)
            {
                Debug.LogError(
                    "[File Save Storage Test] " +
                    "Failed to clean previous test file. " +
                    exception);
                return;
            }

            if (storage.Exists())
            {
                Debug.LogError(
                    "[File Save Storage Test] " +
                    "Test file still exists after Delete().");
                return;
            }

            //==================================================
            // Save
            //==================================================

            try
            {
                storage.Save(
                    originalData);
            }
            catch (System.Exception exception)
            {
                Debug.LogError(
                    "[File Save Storage Test] " +
                    "Save failed. " +
                    exception);
                return;
            }

            if (!storage.Exists())
            {
                Debug.LogError(
                    "[File Save Storage Test] " +
                    "Save completed but file does not exist.");
                return;
            }

            //==================================================
            // Load
            //==================================================

            if (!storage.TryLoad(
                    out string loadedData))
            {
                Debug.LogError(
                    "[File Save Storage Test] " +
                    "TryLoad() returned false after Save().");
                return;
            }

            if (loadedData != originalData)
            {
                Debug.LogError(
                    "[File Save Storage Test] " +
                    "Loaded data does not match saved data.");
                return;
            }

            //==================================================
            // Delete
            //==================================================

            try
            {
                storage.Delete();
            }
            catch (System.Exception exception)
            {
                Debug.LogError(
                    "[File Save Storage Test] " +
                    "Delete failed. " +
                    exception);
                return;
            }

            if (storage.Exists())
            {
                Debug.LogError(
                    "[File Save Storage Test] " +
                    "File still exists after Delete().");
                return;
            }

            if (storage.TryLoad(
                    out string dataAfterDelete))
            {
                Debug.LogError(
                    "[File Save Storage Test] " +
                    "TryLoad() returned true after Delete().");
                return;
            }

            if (dataAfterDelete != null)
            {
                Debug.LogError(
                    "[File Save Storage Test] " +
                    "TryLoad() returned unexpected data after Delete().");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[File Save Storage Test] " +
                "✓ COMPLETE — FileSaveStorage successfully " +
                "saved, loaded, detected, and deleted the test file.");
        }
    }
}