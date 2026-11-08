using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

namespace NovaVertexCode.DialogGraphEngine
{
    public class DGEDialogManager
    {
        private const string DefaultDialogsPath =
            "Scripts/Chaosbound/System/Dialogue/Resources/DGEDialogs";

        private string currentDialogFolder = "Blacksmith";
        private string dialogsPath = DefaultDialogsPath;
        private bool isClearingGraph = false;

        public string CurrentDialogFolder => currentDialogFolder;
        public bool IsClearingGraph => isClearingGraph;
        public string DialogsPath => dialogsPath;

        public DGEDialogManager()
        {
            SetupDialogsPath();

            List<string> folders = GetDialogFolders();

            if (folders.Contains("Blacksmith"))
            {
                currentDialogFolder = "Blacksmith";
            }
            else if (folders.Count > 0)
            {
                currentDialogFolder = folders[0];
            }
        }

        private void CreateStartNode(DGEGraphView graphView)
        {
            string folderPath = $"Assets/{dialogsPath}/{currentDialogFolder}";

            CreateFolderStructure(dialogsPath, currentDialogFolder);

            string finalFolderPath = EnsureUniqueFolderPath(folderPath);

            string assetPath = $"{finalFolderPath}/StartNode_{System.Guid.NewGuid()}.asset";

            DGEStartNode startNode = ScriptableObject.CreateInstance<DGEStartNode>();
            startNode.text = "Start";
            startNode.position = Vector2.zero;
            startNode.id = System.Guid.NewGuid().ToString();
            startNode.outputs = new List<DGEBaseNode>();

            AssetDatabase.CreateAsset(startNode, assetPath);
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(startNode);

            graphView.AddElement(graphView.GenerateGraphNode(startNode));
            Debug.Log($"StartNode создан в {assetPath}");
        }

        private void EnsureStartNodeExists(DGEGraphView graphView)
        {
            if (!graphView._nodeCache.Keys.Any(n => n is DGEStartNode))
            {
                CreateStartNode(graphView);
            }
        }

        private void SetupDialogsPath()
        {
            dialogsPath = DefaultDialogsPath;
            CreateFolderStructure(dialogsPath);
        }

        public void SetDialogsPath(string newPath)
        {
            if (string.IsNullOrWhiteSpace(newPath))
            {
                Debug.LogWarning(
                    "[DGE] Dialog path cannot be empty.");

                return;
            }

            if (!newPath.StartsWith(
                DefaultDialogsPath))
            {
                Debug.LogWarning(
                    "[DGE] Invalid dialogue content location: " +
                    newPath);

                return;
            }

            dialogsPath = newPath;

            CreateFolderStructure(dialogsPath);

            AssetDatabase.Refresh();
        }

        public List<string> GetDialogFolders()
        {
            var folders = new List<string>();

            string fullPath = $"Assets/{dialogsPath}";

            if (!Directory.Exists(fullPath))
                return folders;

            foreach (string directory in Directory.GetDirectories(fullPath))
            {
                folders.Add(Path.GetFileName(directory));
            }

            folders.Sort();

            return folders;
        }

        public void SwitchDialog(string newDialogFolder, DGEGraphView graphView, DGEDialogSelection dialogSelection = null)
        {
            currentDialogFolder = newDialogFolder;
            ClearGraph(graphView);
            LoadNodes(graphView);
            EnsureStartNodeExists(graphView);
            UpdateDialogSelector(graphView); // Добавляем обновление DropdownField

            if (dialogSelection != null)
            {
                dialogSelection.FolderPath = $"Assets/{dialogsPath}/{currentDialogFolder}";
            }
        }

        public void CreateNewDialog(string dialogName, DGEGraphView graphView, DGEDialogSelection dialogSelection = null)
        {
            currentDialogFolder = dialogName;
            string folderPath = $"Assets/{dialogsPath}/{currentDialogFolder}";
            CreateFolderStructure(dialogsPath, currentDialogFolder);

            CreateStartNode(graphView);
            ClearGraph(graphView);
            UpdateDialogSelector(graphView);
            LoadNodes(graphView);

            if (dialogSelection != null)
            {
                dialogSelection.FolderPath = folderPath;
            }
        }

        public void RefreshGraph(DGEGraphView graphView)
        {
            ClearGraph(graphView);
            UpdateDialogSelector(graphView);
            LoadNodes(graphView);
            EnsureStartNodeExists(graphView);
        }

        public void SaveNode(DGEBaseNode node, string dialogFolder)
        {
            if (string.IsNullOrEmpty(dialogFolder))
            {
                dialogFolder = "Default";
            }

            string folderPath = $"Assets/{dialogsPath}/{dialogFolder}";
            CreateFolderStructure(dialogsPath, dialogFolder);

            string path = $"{folderPath}/{node.id}.asset";
            if (!AssetDatabase.Contains(node))
            {
                AssetDatabase.CreateAsset(node, path);
            }
            else
            {
                EditorUtility.SetDirty(node);
                AssetDatabase.SaveAssets();
            }
        }

        public string CreateNodeAsset(DialogType dialogType, string actorName, string text, Vector2 position)
        {
            if (dialogType == DialogType.Start)
            {
                Debug.LogWarning("Manual creation of StartNode is not allowed.");
                return null;
            }

            string folderPath = $"Assets/{dialogsPath}/{currentDialogFolder}";
            CreateFolderStructure(dialogsPath, currentDialogFolder);

            string assetPath;
            int index = 0;
            do
            {
                assetPath = $"{folderPath}/{dialogType}_{System.Guid.NewGuid()}.asset";
                index++;
            } while (AssetDatabase.LoadAssetAtPath<DGEBaseNode>(assetPath) != null);

            DGEBaseNode newNode;
            try
            {
                if (dialogType == DialogType.Text)
                {
                    newNode = ScriptableObject.CreateInstance<DGETextNode>();
                    newNode.actorName = actorName;
                    newNode.text = text;
                }
                else if (dialogType == DialogType.Action)
                {
                    newNode = ScriptableObject.CreateInstance<DGEActionNode>();
                }
                else if (dialogType == DialogType.Random)
                {
                    newNode = ScriptableObject.CreateInstance<DGERandomNode>();
                    ((DGERandomNode)newNode).probabilities = new float[0];
                }
                else
                {
                    Debug.LogError($"Failed to create node: {dialogType} not found");
                    return null;
                }

                newNode.position = position;
                newNode.id = System.Guid.NewGuid().ToString();
                newNode.outputs = new List<DGEBaseNode>();

                AssetDatabase.CreateAsset(newNode, assetPath);
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(newNode);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create node: {ex.Message}");
                return null;
            }

            return assetPath;
        }

        private void LoadNodes(DGEGraphView graphView)
        {
            graphView._nodeCache.Clear();
            string folderPath = $"Assets/{dialogsPath}/{currentDialogFolder}";
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                string[] assets = AssetDatabase.FindAssets("t:DGEBaseNode", new[] { folderPath });
                foreach (var guid in assets)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    DGEBaseNode node = AssetDatabase.LoadAssetAtPath<DGEBaseNode>(path);
                    if (node != null)
                    {
                        graphView.AddElement(graphView.GenerateGraphNode(node));
                    }
                }
            }
        }

        private void ClearGraph(DGEGraphView graphView)
        {
            isClearingGraph = true;
            graphView.DeleteElements(graphView.graphElements.ToList());
            graphView._nodeCache.Clear();
            isClearingGraph = false;
        }

        private void UpdateDialogSelector(DGEGraphView graphView)
        {
            var dialogSelector = graphView.Q<DropdownField>();
            if (dialogSelector != null)
            {
                dialogSelector.choices = GetDialogFolders();
                if (!dialogSelector.choices.Contains(currentDialogFolder))
                {
                    dialogSelector.choices.Add(currentDialogFolder);
                }
                dialogSelector.SetValueWithoutNotify(currentDialogFolder); // Используем SetValueWithoutNotify
            }
        }

        private void CreateFolderStructure(string relativePath, string dialogFolder = null)
        {
            string[] folders = relativePath.Split('/');
            string currentPath = "Assets";

            foreach (string folder in folders)
            {
                string nextPath = $"{currentPath}/{folder}";
                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folder);
                }
                currentPath = nextPath;
            }

            if (!string.IsNullOrEmpty(dialogFolder))
            {
                string dialogPath = $"{currentPath}/{dialogFolder}";
                if (!AssetDatabase.IsValidFolder(dialogPath))
                {
                    AssetDatabase.CreateFolder(currentPath, dialogFolder);
                }
            }

            AssetDatabase.Refresh();
        }

        private string EnsureUniqueFolderPath(string folderPath)
        {
            int index = 0;
            string finalFolderPath = folderPath;
            bool folderExists = AssetDatabase.IsValidFolder(finalFolderPath);

            if (folderExists)
            {
                string[] assets = AssetDatabase.FindAssets("", new[] { finalFolderPath });
                if (assets.Length > 0)
                {
                    do
                    {
                        finalFolderPath = $"{folderPath}_{index}";
                        folderExists = AssetDatabase.IsValidFolder(finalFolderPath);
                        if (!folderExists)
                        {
                            AssetDatabase.CreateFolder($"Assets/{dialogsPath}", $"{currentDialogFolder}_{index}");
                        }
                        index++;
                    } while (folderExists && index < 100);
                }
            }

            return finalFolderPath;
        }
    }
}
