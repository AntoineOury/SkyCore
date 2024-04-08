using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaToConsiderMouseInsideInventory : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rect;

    private static List<RectTransform> _rects = new();

    private void OnEnable()
    {
        _rects.Add(_rect);
    }

    private void OnDisable()
    {
        _rects.Remove(_rect);
    }

    public static bool MouseIsInsideInventory()
    {
        for (int i = 0; i < _rects.Count; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_rects[i], Input.mousePosition))
            {
                return true;
            }
        }
        return false;
    }
}
