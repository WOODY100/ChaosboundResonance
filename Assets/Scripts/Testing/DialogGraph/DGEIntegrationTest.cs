using System.Collections;
using NovaVertexCode.DialogGraphEngine;
using UnityEngine;
using UnityEngine.Events;

public class DGEIntegrationTest : MonoBehaviour
{
    [SerializeField]
    private DGEDialogSelection dialogSelection;

    private DGEBaseNode currentNode;

    private IEnumerator Start()
    {
        // Esperar a que DGEDialogSelection termine su inicialización.
        yield return null;

        if (dialogSelection == null)
        {
            Debug.LogError("[DGE Test] No se ha asignado DGEDialogSelection.");
            yield break;
        }

        // Garantizar que los eventos estén inicializados.
        if (dialogSelection.onUpdateData == null)
            dialogSelection.onUpdateData = new UnityEvent<DGEBaseNode>();

        if (dialogSelection.onDialogEnd == null)
            dialogSelection.onDialogEnd = new UnityEvent();

        if (dialogSelection.onFolderPathChanged == null)
            dialogSelection.onFolderPathChanged = new UnityEvent<string>();

        // Escuchar los eventos del motor.
        dialogSelection.onUpdateData.AddListener(OnNodeUpdated);
        dialogSelection.onDialogEnd.AddListener(OnDialogEnd);

        // Seleccionar nuestro diálogo.
        dialogSelection.FolderPath =
            DGEDialogSelection.DefaultFolderPath + "/CB_Dialogue_Test_01";

        Debug.Log(
            "[DGE Test] Grafo seleccionado: " +
            dialogSelection.FolderPath
        );

        Debug.Log(
            "[DGE Test] Instancia DGEDialogSelection: " +
            dialogSelection.GetInstanceID()
        );
    }

    private void OnNodeUpdated(DGEBaseNode node)
    {
        Debug.Log(
            "[DGE Test] Evento recibido. Instancia: " +
            dialogSelection.GetInstanceID() +
            " | FolderPath: " +
            dialogSelection.FolderPath
        );

        if (node == null)
        {
            Debug.LogError("[DGE Test] Nodo nulo.");
            return;
        }

        currentNode = node;

        Debug.Log("[DGE Test] Tipo: " + node.GetType().Name);
        Debug.Log("[DGE Test] Actor: " + node.actorName);

        Debug.Log(
            "[DGE Test] Texto: " +
            (node.text == null ? "<NULL>" : "'" + node.text + "'")
        );

        int outputCount = node.outputs != null
            ? node.outputs.Count
            : 0;

        Debug.Log("[DGE Test] Cantidad de salidas: " + outputCount);

        if (node.outputs == null)
            return;

        for (int i = 0; i < node.outputs.Count; i++)
        {
            DGEBaseNode output = node.outputs[i];

            if (output is DGEActionNode action)
            {
                Debug.Log(
                    "[DGE Test] Opción " + i + ": " + action.text
                );
            }
            else
            {
                Debug.Log(
                    "[DGE Test] Salida " + i + ": " +
                    (output != null ? output.GetType().Name : "<NULL>")
                );
            }
        }
    }

    private void OnDialogEnd()
    {
        Debug.Log("[DGE Test] Conversación finalizada.");
    }

    private void OnDestroy()
    {
        if (dialogSelection == null)
            return;

        dialogSelection.onUpdateData?.RemoveListener(OnNodeUpdated);
        dialogSelection.onDialogEnd?.RemoveListener(OnDialogEnd);
    }

    [ContextMenu("DGE Test/Select First Option")]
    private void SelectFirstOption()
    {
        SelectOption(0);
    }

    [ContextMenu("DGE Test/Select Second Option")]
    private void SelectSecondOption()
    {
        SelectOption(1);
    }

    private void SelectOption(int index)
    {
        if (dialogSelection == null)
        {
            Debug.LogError("[DGE Test] Dialog Selection no asignado.");
            return;
        }

        if (currentNode == null || currentNode.outputs == null)
        {
            Debug.LogWarning("[DGE Test] No hay un nodo actual con salidas.");
            return;
        }

        if (index < 0 || index >= currentNode.outputs.Count)
        {
            Debug.LogWarning("[DGE Test] Índice de opción inválido.");
            return;
        }

        DGEBaseNode selectedNode = currentNode.outputs[index];

        if (!(selectedNode is DGEActionNode))
        {
            Debug.LogWarning("[DGE Test] La salida seleccionada no es un Action Node.");
            return;
        }

        Debug.Log("[DGE Test] Seleccionando opción: " + selectedNode.text);

        dialogSelection.SelectAction(selectedNode);
    }

    [ContextMenu("DGE Test/Select Third Option")]
    private void SelectThirdOption()
    {
        SelectOption(2);
    }

    [ContextMenu("DGE Test/Select Finalize")]
    private void SelectFinalize()
    {
        SelectOption(1);
    }
}