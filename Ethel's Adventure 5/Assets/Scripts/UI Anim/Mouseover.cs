using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Mouseover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Mouseover Events")]
    public UnityEvent onMouseEnter;
    public UnityEvent onMouseExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseExit?.Invoke();
    }
}
