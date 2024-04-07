using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaiseAkEventComponentOnPointerEnter : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private AkEvent _event;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _event.HandleEvent(null);
    }
}
