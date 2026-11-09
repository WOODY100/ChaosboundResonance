using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace NovaVertexCode.DialogGraphEngine
{
    public class DGEGraphToolbar
    {
        private readonly DGEGraphView graphView;
        private readonly DGEDialogManager dialogManager;

        public DGEGraphToolbar(DGEGraphView graphView, DGEDialogManager dialogManager)
        {
            this.graphView = graphView;
            this.dialogManager = dialogManager;

            CreateToolbar();
        }

        private void CreateToolbar()
        {
            var toolbar = new Toolbar();
            toolbar.style.backgroundColor = new StyleColor(new Color(0.18f, 0.18f, 0.18f));
            toolbar.style.paddingLeft = 5;
            toolbar.style.paddingRight = 5;
            toolbar.style.paddingTop = 0;
            toolbar.style.paddingBottom = 0;

            var dialogSelector = new DropdownField
            {
                name = "dge-dialog-selector",
                label = "Dialog:",
                choices = dialogManager.GetDialogFolders(),
                value = dialogManager.CurrentDialogFolder
            };
            dialogSelector.style.minWidth = 100;
            dialogSelector.RegisterValueChangedCallback(evt =>
            {
                dialogManager.SwitchDialog(evt.newValue, graphView);
            });
            toolbar.Add(dialogSelector);

            var createDialogButton = new ToolbarButton(() => ShowCreateDialogWindow())
            {
                text = "Create Dialog"
            };
            createDialogButton.style.marginLeft = 5;
            toolbar.Add(createDialogButton);

            var refreshButton = new ToolbarButton(() => dialogManager.RefreshGraph(graphView))
            {
                text = "Refresh"
            };
            refreshButton.style.marginLeft = 5;
            toolbar.Add(refreshButton);

            graphView.Add(toolbar);
        }

        private void ShowCreateDialogWindow()
        {
            var window = EditorWindow.GetWindow<DGECreateDialogWindow>("Create New Dialog");
            window.minSize = new Vector2(250, 100);
            window.maxSize = new Vector2(250, 100);
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 100);
            window.Init(graphView);
            window.Show();
        }
    }
}