using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    private GameObject _inventorySlotPrefab;
    [SerializeField]
    private InventorySlot[] _uniqueSlots;
    [SerializeField]
    private Transform _additionalSlotsParent;
    [SerializeField]
    private int _numberOfAdditionalSlots;

    private InventorySlot[] _slots;

    private void Awake()
    {
        InventoryDragAndDrop dragAndDrop = Inventory.Instance.DragAndDrop;
        Transform parentDuringDragAndDrop = Inventory.Instance.ParentDuringDragAndDrop;

        if (dragAndDrop == null)
        {
            throw new System.Exception("dragAndDrop is null.");
        }
        if (parentDuringDragAndDrop == null)
        {
            throw new System.Exception("parentDuringDragAndDrop is null.");
        }

        _slots = new InventorySlot[_uniqueSlots.Length + _numberOfAdditionalSlots];

        for (int i = 0; i < _uniqueSlots.Length; i++)
        {
            _uniqueSlots[i].InitializeAfterInstantiate(ItemIdentity.ItemSortType.None, parentDuringDragAndDrop, dragAndDrop);
            _slots[i] = _uniqueSlots[i];
        }

        for (int i = _uniqueSlots.Length; i < _uniqueSlots.Length + _numberOfAdditionalSlots; i++)
        {
            GameObject instantiated = Instantiate(_inventorySlotPrefab, _additionalSlotsParent);
            _slots[i] = instantiated.GetComponentInChildren<InventorySlot>();
            _slots[i].InitializeAfterInstantiate(ItemIdentity.ItemSortType.None, parentDuringDragAndDrop, dragAndDrop);
        }
    }

    public void OnCloseButton()
    {
        if (AnySlotInThisContainerIsBeingDragged())
        {
            Inventory.Instance.DragAndDrop.TryReturnDraggedItemToItsSlot();
        }
    }

    private bool AnySlotInThisContainerIsBeingDragged()
    {
        InventorySlot beingDragged = Inventory.Instance.DragAndDrop.BeingDragged;
        if (beingDragged == null)
        {
            return false;
        }
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == beingDragged)
            {
                return true;
            }
        }
        return false;
    }
}
