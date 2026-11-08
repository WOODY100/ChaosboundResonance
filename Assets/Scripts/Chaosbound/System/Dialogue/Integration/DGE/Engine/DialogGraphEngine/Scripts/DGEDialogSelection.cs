using UnityEngine;
using UnityEngine.Events;

namespace NovaVertexCode.DialogGraphEngine
{
    public class DGEDialogSelection : MonoBehaviour
    {
        public const string ResourcesPrefix =
            "Assets/Scripts/Chaosbound/System/Dialogue/Resources/";

        public const string DefaultFolderPath =
            ResourcesPrefix + "DGEDialogs/Blacksmith";

        [SerializeField]
        private string folderPath = DefaultFolderPath;

        private DGEBaseNode currentData;
        private bool dialogLoaded;

        public UnityEvent<DGEBaseNode> onUpdateData =
            new UnityEvent<DGEBaseNode>();

        public UnityEvent onDialogEnd =
            new UnityEvent();

        [HideInInspector]
        public UnityEvent<string> onFolderPathChanged =
            new UnityEvent<string>();

        public string FolderPath
        {
            get => folderPath;
            set
            {
                if (folderPath == value)
                    return;

                folderPath = value;

                if (onFolderPathChanged != null)
                    onFolderPathChanged.Invoke(folderPath);

                LoadStartNode();
            }
        }

        public bool IsDialogLoaded => dialogLoaded;

        private void Awake()
        {
            EnsureEventsInitialized();
            LoadStartNode();
        }

        private void EnsureEventsInitialized()
        {
            if (onUpdateData == null)
                onUpdateData = new UnityEvent<DGEBaseNode>();

            if (onDialogEnd == null)
                onDialogEnd = new UnityEvent();

            if (onFolderPathChanged == null)
                onFolderPathChanged = new UnityEvent<string>();
        }

        /// <summary>
        /// Inicia la conversación utilizando el nodo
        /// que se preparó al cargar el grafo.
        /// </summary>
        public void StartDialog()
        {
            if (!dialogLoaded || currentData == null)
            {
                Debug.LogWarning(
                    "[DGE] Cannot start dialog. No valid start node was loaded.",
                    this);

                return;
            }

            onUpdateData.Invoke(currentData);
        }

        /// <summary>
        /// Avanza desde el ActionNode seleccionado.
        /// </summary>
        public void SelectAction(DGEBaseNode node)
        {
            if (!(node is DGEActionNode actionNode))
                return;

            if (!(currentData is DGETextNode textNode))
            {
                Debug.LogWarning(
                    "[DGE] Cannot select an action because the current node is not a TextNode.",
                    this);

                return;
            }

            if (textNode.outputs == null ||
                !textNode.outputs.Contains(actionNode))
            {
                Debug.LogWarning(
                    "[DGE] The selected action is not connected to the current TextNode.",
                    this);

                return;
            }

            currentData = actionNode.NextNode();

            ResolveRandomNodes();

            if (currentData != null)
            {
                onUpdateData.Invoke(currentData);
            }
            else
            {
                EndDialog();
            }
        }

        /// <summary>
        /// Continúa el diálogo desde el nodo actual utilizando
        /// el índice de salida indicado.
        /// </summary>
        public bool ContinueFromCurrentNode(
            DGEBaseNode expectedCurrentNode,
            int outputIndex)
        {
            if (!dialogLoaded ||
                currentData == null ||
                expectedCurrentNode == null)
            {
                return false;
            }

            if (currentData != expectedCurrentNode)
            {
                Debug.LogWarning(
                    "[DGE] Cannot continue. The current node has changed.",
                    this);

                return false;
            }

            if (expectedCurrentNode.outputs == null ||
                outputIndex < 0 ||
                outputIndex >= expectedCurrentNode.outputs.Count)
            {
                Debug.LogWarning(
                    "[DGE] Invalid output index: " + outputIndex,
                    this);

                return false;
            }

            currentData = expectedCurrentNode.outputs[outputIndex];

            ResolveRandomNodes();

            if (currentData != null)
            {
                onUpdateData.Invoke(currentData);
            }
            else
            {
                EndDialog();
            }

            return true;
        }

        private void ResolveRandomNodes()
        {
            while (currentData is DGERandomNode randomNode)
            {
                currentData = randomNode.NextNode();
            }
        }

        /// <summary>
        /// Finaliza la sesión actual.
        /// </summary>
        public void EndDialog()
        {
            currentData = null;
            dialogLoaded = false;

            onDialogEnd.Invoke();
        }

        /// <summary>
        /// Carga el StartNode del grafo asignado.
        /// No inicia la conversación ni dispara onUpdateData.
        /// </summary>
        public bool LoadStartNode()
        {
            currentData = null;
            dialogLoaded = false;

            string resourcesPath = GetResourcesPath(folderPath);

            if (string.IsNullOrEmpty(resourcesPath))
            {
                Debug.LogWarning(
                    "[DGE] Invalid dialogue folder path: " + folderPath,
                    this);

                return false;
            }

            DGEStartNode[] startNodes =
                Resources.LoadAll<DGEStartNode>(resourcesPath);

            if (startNodes == null || startNodes.Length == 0)
            {
                Debug.LogWarning(
                    "[DGE] No StartNode found in Resources path: " +
                    resourcesPath,
                    this);

                return false;
            }

            DGEStartNode startNode = startNodes[0];

            currentData = startNode.NextNode();

            ResolveRandomNodes();

            if (currentData == null)
            {
                Debug.LogWarning(
                    "[DGE] StartNode is not connected. Folder: " +
                    folderPath,
                    this);

                return false;
            }

            dialogLoaded = true;

            return true;
        }

        private string GetResourcesPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            path = path.Replace("\\", "/").Trim();

            if (path.StartsWith(ResourcesPrefix))
            {
                path = path.Substring(ResourcesPrefix.Length);
            }

            return path.Trim('/');
        }
    }
}