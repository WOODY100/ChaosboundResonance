using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NovaVertexCode.DialogGraphEngine
{
    public class DGEGraphWindow : EditorWindow
    {
        public DGEGraphView graphView;

        [MenuItem("Window/Dialog Graph")]
        public static void OpenGraphViewWindow()
        {
            var window = GetWindow<DGEGraphWindow>();
            window.titleContent = new GUIContent("Dialog Graph");
            window.Show();
        }

        private void OnEnable()
        {
            graphView = new DGEGraphView()
            {
                name = "Dialog Graph",
            };

            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }
    }
}