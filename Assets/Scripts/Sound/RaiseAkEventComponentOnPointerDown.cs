using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaiseAkEventComponentOnPointerDown : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private AkEvent _event;

    public void OnPointerDown(PointerEventData eventData)
    {
        _event.HandleEvent(null);
    }
}
