using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronCooking : MonoBehaviour
{
    [SerializeField]
    private CookingSettings _settings;
    [SerializeField]
    private InventorySlot _ingredientASlot;
    [SerializeField]
    private InventorySlot _ingredientBSlot;
    [SerializeField]
    private InventorySlot _productSlot;
    [SerializeField]
    private RectTransform _cookingProgressMaskRect;
    [SerializeField]
    private GameObject _uiToggle;

    private ItemIdentity _productBeingCooked;
    private float _cookingProgressMaskRectFullSize;
    private float _cookProgressFraction;


    private void Awake()
    {
        _cookingProgressMaskRectFullSize = _cookingProgressMaskRect.rect.width;
        UpdateCookingProgressMaskRectSize();
        SetSlotRequirements();
    } 

    private void SetSlotRequirements()
    {
        // Drag & drop can move food ingredients into the ingredient slots.
        _ingredientASlot.RequirementForMovingItemInViaUI = IngredientSlotRequirement;
        _ingredientBSlot.RequirementForMovingItemInViaUI = IngredientSlotRequirement;

        // Drag & drop can't move anything into the product slot.
        _productSlot.RequirementForMovingItemInViaUI = (id) => false;
    }

    private static bool IngredientSlotRequirement(ItemIdentity id)
    {
        // The player can only move food ingredients into the ingredient slots.
        FoodItemIdentity foodId = id as FoodItemIdentity;
        if (foodId == null)
        {
            return false;
        }
        return foodId.IsIngredientInCauldron;
    }


    public void OnCookButton()
    {
        if (!IngredientSlotsHaveItems())
        {
            return;
        }

        ItemIdentity newProduct = DetermineProduct();
        if (_productBeingCooked == newProduct)
        {
            return;
        }

        if (!TryEnsureProductSlotCanTakeIn(newProduct))
        {
            return;
        }
        
        _productBeingCooked = newProduct;
        _cookProgressFraction = 0f;
    }


    private void Update()
    {
        // This won't do anything if _productBeingCooked is null.
        float priorProgress = _cookProgressFraction;
        TryContinueCooking();
        if (_cookProgressFraction != priorProgress)
        {
            UpdateCookingProgressMaskRectSize();
        }
    }

    private void TryContinueCooking()
    {
        if (!CanContinueCooking())
        {
            _productBeingCooked = null;
            _cookProgressFraction = 0f;
            return;
        }

        _cookProgressFraction += Time.deltaTime / _settings.CookingTime;
        _cookProgressFraction = Mathf.Min(1f, _cookProgressFraction);

        if (_cookProgressFraction == 1)
        {
            TryFinishCooking();
        }
    }

    private void TryFinishCooking()
    {
        if (!TryEnsureProductSlotCanTakeIn(_productBeingCooked))
        {
            return;
        }

        if (_productSlot._itemStack == null)
        {
            _productSlot._itemStack = ItemStack.ProduceObject(_productBeingCooked, 1);
        }
        else
        {
            _productSlot._itemStack.amount++;
        }
        _ingredientASlot._itemStack.amount--;
        _ingredientBSlot._itemStack.amount--;

        _productSlot.OnItemStackChanged();
        _ingredientASlot.OnItemStackChanged();
        _ingredientBSlot.OnItemStackChanged();

        _productBeingCooked = null;
        _cookProgressFraction = 0f;
    }




    private void UpdateCookingProgressMaskRectSize()
    {
        float size = _cookProgressFraction * _cookingProgressMaskRectFullSize;
        _cookingProgressMaskRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
    }

    private bool TryEnsureProductSlotCanTakeIn(ItemIdentity id)
    {
        if (!ProductSlotCanTakeIn(id))
        {
            if (!_uiToggle.activeSelf)
            {
                // Don't move product into the player's inventory if the cauldron's ui isn't active.
                return false;
            }

            TryMoveProductIntoPlayerInventory();
            if (!ProductSlotCanTakeIn(id))
            {
                return false;
            }
        }
        return true;
    }

    private void TryMoveProductIntoPlayerInventory()
    {
        if (_productSlot._itemStack != null)
        {
            Inventory.Instance.TakeInAsManyAsFit(_productSlot._itemStack);
            _productSlot.OnItemStackChanged();
        }
    }

    private bool IngredientSlotsHaveItems()
    {
        return _ingredientASlot._itemStack != null && _ingredientBSlot._itemStack != null;
    }

    private bool ProductSlotIsFull()
    {
        return _productSlot._itemStack != null && _productSlot._itemStack.IsStackFull;
    }

    private bool ProductSlotCanTakeIn(ItemIdentity id)
    {
        ItemStack inProductSlot = _productSlot._itemStack;
        return inProductSlot == null || (inProductSlot.identity == id && !inProductSlot.IsStackFull);
    }

    private bool CanContinueCooking()
    {
        return _productBeingCooked != null
            && IngredientSlotsHaveItems()
            && !ProductSlotIsFull()
            && ProductSlotCanTakeIn(_productBeingCooked)
            && _productBeingCooked == DetermineProduct();
    }

    private ItemIdentity DetermineProduct() 
    {
        if (_ingredientASlot._itemStack == null)
        {
            throw new System.Exception("This shouldn't run.");
        }
        if (_ingredientBSlot._itemStack == null)
        {
            throw new System.Exception("This shouldn't run. #2");
        }

        return _settings.DetermineProduct(_ingredientASlot._itemStack.identity
            , _ingredientBSlot._itemStack.identity);
    }


    

    
}
