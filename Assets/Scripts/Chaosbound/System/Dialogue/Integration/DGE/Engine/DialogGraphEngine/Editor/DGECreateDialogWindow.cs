using UnityEditor;
using UnityEngine.UIElements;

namespace NovaVertexCode.DialogGraphEngine
{
    public class DGECreateDialogWindow : EditorWindow
    {
        private DGEGraphView graphView;
        private TextField textField;

        public void Init(DGEGraphView view)
        {
            graphView = view;
            SetupUI();
        }

        private void SetupUI()
        {
            rootVisualElement.Clear();

            textField = new TextField("Dialog Name:")
            {
                value = ""
            };
            textField.style.marginTop = 10;
            textField.style.marginBottom = 10;
            rootVisualElement.Add(textField);

            var createButton = new Button(() =>
            {
                string dialogName = textField.value.Trim();
                if (!string.IsNullOrEmpty(dialogName))
                {
                    graphView.dialogManager.CreateNewDialog(dialogName, graphView);
                    Close();
                }
            })
            {
                text = "Create"
            };
            createButton.style.marginTop = 10;
            rootVisualElement.Add(createButton);
        }
    }
}