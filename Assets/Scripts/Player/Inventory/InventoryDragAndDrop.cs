using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Player.Motion;
using Player.View;


/// <summary>
/// Handles dragging items between inventory slots.
/// </summary>
public class InventoryDragAndDrop
{
    private const float GENERATOR_DROPPABLE_RADIUS_AROUND_ISLAND_HEARTS = 3f;
    private const float TOSS_DIRECTION_MAX_ANGLE_OFFSET = 1f;

    private InputAction _click;
    private bool _stillHoveringOverSlotBeingDragged;
    private InventorySlot _beingDragged;

    private List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private PointerEventData _eventData;
    private EventSystem _eventSystem;

    private LayerMask _jellyFeedingLayerMask = 1 << LayerMask.NameToLayer("Jellies");


    private Transform _playerTransform;
    private FoodItemIdentity _berryItemIdentity;

    public InventorySlot BeingDragged => _beingDragged;

    public InventoryDragAndDrop(FoodItemIdentity berryItemIdentity)
    {
        _berryItemIdentity = berryItemIdentity;
        _click = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().actions.FindAction("ClickForDragAndDrop", true);
        _click.started += OnClickStart;
        _click.canceled += OnClickCancel;
        _click.Disable();

        _playerTransform = PlayerMovement.Instance.transform;
    }

    public void EnableInput()
    {
        _click.Enable();
    }

    public void DisableInputAndStop()
    {
        TryReturnDraggedItemToItsSlot();
        _click.Disable();
    }

    public void TryReturnDraggedItemToItsSlot()
    {
        if (_beingDragged != null)
        {
            TryMoveDraggedItemToSlot(_beingDragged, true);
        }
    }

    private void OnClickStart(InputAction.CallbackContext context)
    {
        if (Inventory.Instance.IgnoreInput.AnyReasons)
        {
            return;
        }

        // There are 2 ways to click and drag between slots:
        // 1. Press, release without moving the mouse outside the slot, then move the mouse to a different slot and press again.
        // 2. Press, move the mouse to a different slot, and release.
        if (_beingDragged == null)
        {
            TryBeginDrag();
        }
        else
        {
            TryFinishDrag();
        }
    }

    private void OnClickCancel(InputAction.CallbackContext context)
    {
        if (Inventory.Instance.IgnoreInput.AnyReasons)
        {
            return;
        }

        if (_beingDragged != null)
        {
            CheckStoppedHoveringOverOriginalSlot();
            if (!_stillHoveringOverSlotBeingDragged && FindInventorySlotBelowMouse() != null)
            {
                TryFinishDrag();
            }
        }
    }

   

    public void OnLateUpdate()
    {
        if (_beingDragged != null)
        {
            if (Inventory.Instance.IgnoreInput.AnyReasons)
            {
                TryMoveDraggedItemToSlot(_beingDragged, true);
            }
            else
            {
                _beingDragged.FollowMouse();
            }
        }

        CheckStoppedHoveringOverOriginalSlot();
    }

    private void CheckStoppedHoveringOverOriginalSlot()
    {
        if (_stillHoveringOverSlotBeingDragged && FindInventorySlotBelowMouse() != _beingDragged)
        {
            _stillHoveringOverSlotBeingDragged = false;
        }
    }

    



    private void TryBeginDrag()
    {
        InventorySlot slotBelowMouse = FindInventorySlotBelowMouse();
        if (slotBelowMouse != null && slotBelowMouse.IsNotEmpty)
        {
            _beingDragged = slotBelowMouse;
            _beingDragged.StartItemDrag();
            _stillHoveringOverSlotBeingDragged = true;
        }
    }

    private void TryFinishDrag()
    {
        if (_beingDragged._itemStack == null)
        {
            throw new System.Exception("_beingDragged._itemStack is null. This shouldn't be possible");
        }

        if (!AreaToConsiderMouseInsideInventory.MouseIsInsideInventory())
        {
            FinishDragOutsideInventory();
        }
        else
        {
            InventorySlot slotBelowMouse = FindInventorySlotBelowMouse();
            if (slotBelowMouse != null 
                && !InventoryInfoGetter.UnmatchedSortTypes(_beingDragged._itemStack.identity.SortType, slotBelowMouse.SortType))
            {
                if (slotBelowMouse._itemStack == null || !InventoryInfoGetter.UnmatchedSortTypes(slotBelowMouse._itemStack.identity.SortType
                    , _beingDragged.SortType))
                {
                    TryMoveDraggedItemToSlot(slotBelowMouse);
                }
            }
        }
    }

    private void FinishDragOutsideInventory()
    {
        if (!InteractableWithUIMode.AnyInteracting)
        {
            TryTossItem();
            return;
        }

        // Try to give an item to the interactable
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, _jellyFeedingLayerMask))
        {
            InteractableWithUIMode interactable = hit.collider.GetComponent<InteractableWithUIMode>();
            if (interactable == InteractableWithUIMode.CurrentlyInteracting
                && interactable.InventoryInteraction(_beingDragged._itemStack.identity))
            {
                _beingDragged._itemStack.amount--;
                _beingDragged.OnItemStackChanged();
            }
        }
    }

    private void TryMoveDraggedItemToSlot(InventorySlot moveTo, bool throwIfFails = false)
    {
        if (moveTo == _beingDragged)
        {
            _beingDragged.StopItemDrag();
            _beingDragged = null;
            return;
        }

        InventorySlot wasBeingDragged = _beingDragged; // the next line can make _beingDragged null
        if (Inventory.TryMoveItemBetweenSlots(_beingDragged, moveTo))
        {
            wasBeingDragged.StopItemDrag();
            _beingDragged = null;
        }
        else if (throwIfFails)
        {
            throw new System.Exception("Couldn't move the dragged item to slot");
        }
    }



    public void CheckDraggedStackNowEmpty()
    {
        if (_beingDragged != null && _beingDragged._itemStack == null)
        {
            _beingDragged.StopItemDrag();
            _beingDragged = null;
        }
    }

    /// <summary>
    /// Toss an item if it is dragged out of the inventory.
    /// </summary>
    private void TryTossItem()
    {
        if (_beingDragged._itemStack.amount <= 0)
        {
            throw new System.Exception("item stack amount is <= 0 in TossItem. amount: " + _beingDragged._itemStack.amount);
        }

        // If the item is a generator and the closest island heart is too far away, return.
        if (_beingDragged._itemStack.identity is GeneratorItemIdentity)
        {
            GameObject[] IslandHeartPos = GameObject.FindGameObjectsWithTag("IslandHeart");
            float closestIslandHeart = Mathf.Infinity;

            foreach (GameObject IslandHeart in IslandHeartPos)
            {
                float distance = (IslandHeart.transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).magnitude;
                if (distance < closestIslandHeart)
                {
                    closestIslandHeart = distance;
                }
            }

            if (closestIslandHeart > GENERATOR_DROPPABLE_RADIUS_AROUND_ISLAND_HEARTS)
            {
                return;
            }
        }

        // Spawn the item and toss it.

        GameObject itemPrefab = _beingDragged._itemStack.identity.ItemPrefab;
        Vector3 spawnPos = FirstPersonView.Instance.CameraTarget.position - .1f * Vector3.up;
        GameObject spawned = Object.Instantiate(itemPrefab, spawnPos, _playerTransform.rotation);
        Vector3 tossDirection = FirstPersonView.Instance.CameraTarget.forward;

        // Randomize the toss direction very very slightly. Otherwise when you toss a bunch of berries, they pile up in a line
        // 1 berry wide because their colliders are perfectly aligned.
        float angleOffset = Random.Range(0, TOSS_DIRECTION_MAX_ANGLE_OFFSET);
        float distanceOffset = angleOffset * Mathf.Deg2Rad * Mathf.Sqrt(2) / (.25f * Mathf.PI);
        // stack exchange how to find a random unit vector orthogonal to a random unit vector in 3d
        Vector3 r = Random.insideUnitSphere;
        Vector3 u = r - Vector3.Dot(r, tossDirection) * tossDirection;
        tossDirection += distanceOffset * u.normalized;
        tossDirection.Normalize();

        spawned.GetComponent<PickupItem>().TossFromInventory(tossDirection);


        _beingDragged._itemStack.amount--;
        _beingDragged.OnItemStackChanged();
    }





    private InventorySlot FindInventorySlotBelowMouse()
    {
        // source: https://forum.unity.com/threads/how-to-detect-if-mouse-is-over-ui.1025533/
        // is there a better way to do this?

        if (_eventSystem != EventSystem.current)
        {
            _eventSystem = EventSystem.current;

            if (_eventSystem == null)
            {
                return null;
            }

            _eventData = new PointerEventData(_eventSystem);
        }

        _eventData.position = Input.mousePosition;

        _raycastResults.Clear();
        _eventSystem.RaycastAll(_eventData, _raycastResults);

        foreach (RaycastResult raycastResult in _raycastResults)
        {
            InventorySlot result = raycastResult.gameObject.GetComponent<InventorySlot>();
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }


}

