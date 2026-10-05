using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class SkillTreeEmptyAreaClickHandler :
    MonoBehaviour,
    IPointerClickHandler
{
    [SerializeField]
    private SkillTreeVisualController visualController;

    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (visualController == null)
            return;

        GameObject clickedObject =
            eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == null)
            return;

        Button button =
            clickedObject.GetComponentInParent<Button>();

        if (button != null)
            return;

        visualController.ClearSelection();
    }
}