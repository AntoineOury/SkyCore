using Player;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UI.Inventory;
using UnityEngine;

public class HoldingItemHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _helditemVisual;

    public int _heldItemSlotIndex = 0;
    public ItemStack HeldItem { get; private set; }


    private void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            HeldItemHandling(0);
        }
        else if (Input.GetKeyDown("2"))
        {
            HeldItemHandling(1);
        }
        else if (Input.GetKeyDown("3"))
        {
            HeldItemHandling(2);
        }
    }

    private void HeldItemHandling(int slotIndex)
    {
        ItemStack itemStack = InventoryUI.Instance.HotbarUI.GetItemAtSlotIndex(slotIndex);

        if (itemStack != null)
        {
            // Visually Change Model
            _helditemVisual.GetComponent<MeshFilter>().sharedMesh = itemStack.itemInfo.ItemPrefab.GetComponent<MeshFilter>().sharedMesh;
            // Copy Materials Over
            _helditemVisual.GetComponent<MeshRenderer>().sharedMaterials = itemStack.itemInfo.ItemPrefab.GetComponent<MeshRenderer>().sharedMaterials;

            HeldItem = itemStack;
        }
        else
        {
            _helditemVisual.GetComponent<MeshFilter>().sharedMesh = null;
            HeldItem = null;
        }
        // Change Index
        _heldItemSlotIndex = slotIndex;

        Transform selectedGrid = InventoryUI.Instance.HotBarGrid.transform.GetChild(slotIndex);
        InventoryUI.Instance.HotbarHighlight.transform.position = selectedGrid.position;
    }

    public void UpdateHeldItem()
    {
        HeldItemHandling(_heldItemSlotIndex);
    }
}
