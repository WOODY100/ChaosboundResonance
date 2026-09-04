using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButtonVisual : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [SerializeField] private GameObject selectedIcon;

    public void OnSelect(BaseEventData eventData)
    {
        SetSelectedVisual(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetSelectedVisual(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    private void SetSelectedVisual(bool selected)
    {
        if (selectedIcon != null)
            selectedIcon.SetActive(selected);
    }
}