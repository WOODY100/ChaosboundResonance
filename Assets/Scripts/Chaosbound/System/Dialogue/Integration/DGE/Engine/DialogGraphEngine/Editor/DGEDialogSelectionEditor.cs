using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace NovaVertexCode.DialogGraphEngine
{
    [CustomEditor(typeof(DGEDialogSelection))]
    public class DGEDialogSelectionEditor : Editor
    {
        private List<string> folderPaths = new List<string>();
        private List<string> folderNames = new List<string>();
        private int selectedIndex = 0;

        private void OnEnable()
        {
            folderPaths.Clear();
            folderNames.Clear();

            string dialogsPath =
                "Assets/Scripts/Chaosbound/System/Dialogue/Resources/DGEDialogs";

            if (Directory.Exists(dialogsPath))
            {
                string[] directories = Directory.GetDirectories(dialogsPath, "*", SearchOption.AllDirectories);
                foreach (string dir in directories)
                {
                    string relativePath = dialogsPath + dir.Substring(dialogsPath.Length).Replace("\\", "/");
                    folderPaths.Add(relativePath);

                    string folderName = relativePath.Substring(relativePath.LastIndexOf("/") + 1);
                    folderNames.Add(folderName);
                }
            }

            DGEDialogSelection dataManager = (DGEDialogSelection)target;
            selectedIndex = folderPaths.IndexOf(dataManager.FolderPath);
            if (folderPaths.Count == 0)
            {
                selectedIndex = -1;
            }
            else if (selectedIndex == -1 || selectedIndex >= folderPaths.Count)
            {
                selectedIndex = 0;
            }
        }

        public override void OnInspectorGUI()
        {
            DGEDialogSelection dataManager = (DGEDialogSelection)target;

            EditorGUILayout.LabelField("Select Dialog:");
            if (folderNames.Count > 0)
            {
                selectedIndex = EditorGUILayout.Popup(selectedIndex, folderNames.ToArray());
                if (selectedIndex >= folderPaths.Count)
                {
                    selectedIndex = folderPaths.Count - 1;
                }
                if (folderPaths[selectedIndex] != dataManager.FolderPath)
                {
                    dataManager.FolderPath = folderPaths[selectedIndex];
                    EditorUtility.SetDirty(dataManager);
                }
            }
            else
            {
                EditorGUILayout.LabelField("No dialog folders found.");
            }

            if (GUILayout.Button("Open Dialog Graph"))
            {
                if (selectedIndex >= 0 && selectedIndex < folderPaths.Count)
                {
                    string selectedFolderPath = folderPaths[selectedIndex];
                    string folderName = folderNames[selectedIndex];

                    DGEGraphWindow window = EditorWindow.GetWindow<DGEGraphWindow>("Dialog Graph");
                    window.Show();

                    if (window.graphView != null)
                    {
                        window.graphView.dialogManager.SwitchDialog(folderName, window.graphView);
                    }
                    else
                    {
                        Debug.LogWarning("GraphView not found in DGEGraphWindow.");
                    }
                }
                else
                {
                    DGEGraphWindow window = EditorWindow.GetWindow<DGEGraphWindow>("Dialog Graph");
                    window.Show();
                }
            }

            DrawDefaultInspector();
        }
    }
}