using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;


namespace UI.Inventory
{
    public class InventoryBaseUI
    {

        private InventorySlotUI[] _slotUIs;

        private Dictionary<ItemStack, int> _itemsInSlotIndexes = new Dictionary<ItemStack, int>();

        public InventoryBase InventoryOrHotBar { get; private set; }

        private ItemBase.ItemSortType _typeAllowed;

        public InventoryBaseUI(GameObject inventorySlotPrefab, GameObject inventoryOrHotBarGrid
            , InventoryBase inventoryOrHotBar, Transform itemParentDuringDragAndDrop, ItemBase.ItemSortType type)
        {
            InventoryOrHotBar = inventoryOrHotBar;
            InventoryOrHotBar.SetInventoryBaseUI(this);

            _slotUIs = new InventorySlotUI[inventoryOrHotBar.StacksCapacity];

            _typeAllowed = type;
            for (int i = 0; i < _slotUIs.Length; i++)
            {
                GameObject slot = Object.Instantiate(inventorySlotPrefab, inventoryOrHotBarGrid.transform);
                _slotUIs[i] = slot.GetComponentInChildren<InventorySlotUI>();
                _slotUIs[i].InitializeAfterInstantiate(this, itemParentDuringDragAndDrop, _typeAllowed);
            }
        }

        public void OnChangeItem(ItemStack item)
        {
            int index;
            if (!_itemsInSlotIndexes.TryGetValue(item, out index))
            {
                index = GetEmptySlotWithLowestIndex();
                _itemsInSlotIndexes.Add(item, index);
            }
            if (item.amount <= 0)
            {
                _itemsInSlotIndexes.Remove(item);
                _slotUIs[index].ShowItem(null);
            }
            else
            {
                _slotUIs[index].ShowItem(item);
            }
        }

        private int GetEmptySlotWithLowestIndex()
        {
            for (int i = 0; i < _slotUIs.Length; i++)
            {
                if (!_itemsInSlotIndexes.ContainsValue(i))
                {
                    return i;
                }
            }
            throw new System.InvalidOperationException("Couldn't get empty slow with lowest index. This shouldn't be possible.");
        }

        public int GetIndexOfSlot(InventorySlotUI slot)
        {
            for (int i = 0; i < _slotUIs.Length; i++)
            {
                if (slot == _slotUIs[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsNotEmpty(InventorySlotUI slot)
        {
            int index = GetIndexOfSlot(slot);
            return _itemsInSlotIndexes.ContainsValue(index);
        }

        public ItemStack GetItemInSlot(InventorySlotUI slot)
        {
            int index = GetIndexOfSlot(slot);

            foreach (ItemStack item in _itemsInSlotIndexes.Keys)
            {
                if (_itemsInSlotIndexes[item] == index)
                {
                    return item;
                }
            }
            return null;
        }

        public static void MoveItemBetweenSlots(InventorySlotUI from, InventorySlotUI to)
        {
            if (from == to)
            {
                return;
            }

            if(from.SlotType != to.SlotType && from.SlotType != ItemBase.ItemSortType.None && to.SlotType != ItemBase.ItemSortType.None)
            {
                return;
            }


            InventoryBaseUI fromUI = from.InventoryOrHotBarUI;
            InventoryBaseUI toUI = to.InventoryOrHotBarUI;

            Dictionary<ItemStack, int> fromItemsInSlotIndexes = fromUI._itemsInSlotIndexes;
            Dictionary<ItemStack, int> toItemsInSlotIndexes = toUI._itemsInSlotIndexes;

            ItemStack itemToMove = from.ItemStack;
            ItemStack itemInTo = to.ItemStack; // can be null, if drag and drop an item into an empty slot.

            if (itemToMove.itemInfo.SortType != to.SlotType && to.SlotType != ItemBase.ItemSortType.None)
            {
                return;
            }
            else if (itemInTo != null && itemInTo.itemInfo.SortType != from.SlotType && from.SlotType != ItemBase.ItemSortType.None)
            {
                return;
            }


            int fromSlotIndex = from.SlotIndex;
            int toSlotIndex = to.SlotIndex;

            fromItemsInSlotIndexes.Remove(itemToMove);
            if (itemInTo != null)
            {
                toItemsInSlotIndexes.Remove(itemInTo);
            }

            toItemsInSlotIndexes.Add(itemToMove, toSlotIndex);
            if (itemInTo != null)
            {
                fromItemsInSlotIndexes.Add(itemInTo, fromSlotIndex);
            }

            if (fromUI != toUI)
            {
                // Move the item between the sets of items for the different inventory sections.
                // If two items are being swapped, move both.
                InventoryBase.MoveItemBetweenInventorySections(fromUI.InventoryOrHotBar
                    , toUI.InventoryOrHotBar, itemToMove);
                if (itemInTo != null)
                {
                    InventoryBase.MoveItemBetweenInventorySections(toUI.InventoryOrHotBar
                    , fromUI.InventoryOrHotBar, itemInTo);
                }
            }

            from.ShowItem(from.ItemStack);
            to.ShowItem(to.ItemStack);

            HoldingItemHandler.Instance.UpdateHeldItem();
        }

        public ItemStack GetItemAtSlotIndex(int index)
        {
            foreach (ItemStack item in _itemsInSlotIndexes.Keys)
            {
                if (index == _itemsInSlotIndexes[item])
                {
                    return item;
                }
            }
            return null;
        }
    }
}