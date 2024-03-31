using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Player.Motion;
using Player.View;


public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject _hotBarHighlight;
    [SerializeField]
    private GameObject _inventorySlotPrefab;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private GameObject _prefabForItemParentDuringDragAndDrop;
    [SerializeField]
    private Transform _hotbarSectionSlotsParent;
    [SerializeField]
    private Transform _jellySectionSlotsParent;
    [SerializeField]
    private Transform _toolSectionSlotsParent;
    [SerializeField]
    private Transform _resourceSectionSlotsParent;

    [SerializeField]
    private FoodItemIdentity _berryItemIdentity;

    [SerializeField]
    private int _hotBarStacksCapacity = 3;
    [SerializeField]
    private int _resourceSectionStacksCapacity = 12;
    [SerializeField]
    private int _jellySectionStacksCapacity = 15;
    [SerializeField]
    private int _toolSectionStacksCapacity = 9;

    [SerializeField]
    private float _animationDuration = .5f;

    private InventorySection _hotbarSection;
    private InventorySection[] _sections;

    private InputAction _backpackAction;
    public bool _isInBackpackMode;
        
    public InventoryDragAndDrop _dragAndDrop;

    public InventoryDragAndDrop DragAndDrop => _dragAndDrop;

    public Transform ParentDuringDragAndDrop { get; private set; }

    private static Inventory _instance;
    public static Inventory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindGameObjectWithTag("InventoryUI").GetComponent<Inventory>();
                _instance.CheckCreateThingsUsedByContainersToo();
            }
            return _instance;
        }
    }

    public Reasons IgnoreInput { get; private set; } = new();


    public GameObject HotbarHighlight => _hotBarHighlight;
    public Transform HotBarSlotsParent => _hotbarSectionSlotsParent;

    private void CheckCreateThingsUsedByContainersToo()
    {
        if (_dragAndDrop == null)
        {
            ParentDuringDragAndDrop = Instantiate(_prefabForItemParentDuringDragAndDrop).transform;
            _dragAndDrop = new InventoryDragAndDrop(_berryItemIdentity);
        }

    }

    private void Awake()
    {
        CheckCreateThingsUsedByContainersToo();

        _hotbarSection = new InventorySection(_hotBarStacksCapacity, ItemIdentity.ItemSortType.None
            , _inventorySlotPrefab, _hotbarSectionSlotsParent, ParentDuringDragAndDrop, _dragAndDrop);

        InventorySection jellySection = new InventorySection(_jellySectionStacksCapacity, ItemIdentity.ItemSortType.JellyItem
            , _inventorySlotPrefab, _jellySectionSlotsParent, ParentDuringDragAndDrop, _dragAndDrop);
        InventorySection toolSection = new InventorySection(_toolSectionStacksCapacity, ItemIdentity.ItemSortType.Tool
            , _inventorySlotPrefab, _toolSectionSlotsParent, ParentDuringDragAndDrop, _dragAndDrop);
        InventorySection resourceSection = new InventorySection(_resourceSectionStacksCapacity, ItemIdentity.ItemSortType.Resource
           , _inventorySlotPrefab, _resourceSectionSlotsParent, ParentDuringDragAndDrop, _dragAndDrop);

        _sections = new InventorySection[] { _hotbarSection, jellySection, toolSection, resourceSection };

        PlayerInput playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        _backpackAction = playerInput.actions.FindAction("Backpack", true);
        _backpackAction.performed += OnBackpack;
    }

    private void Start()
    {
        HoldingItemHandler.Instance.UpdateHeldItem();
    }

    private void OnEnable() => _backpackAction.Enable();

    private void OnDisable() => _backpackAction.Disable();

    private void Update()
    {
        // The animation should be 1 second long.
        // The InventoryTime parameter is the position in the animation (see the animator state).
        float animationTime = _animator.GetFloat("InventoryTime");
        animationTime += (_isInBackpackMode ? 1 : -1) * Time.deltaTime / _animationDuration;
        animationTime = Mathf.Clamp01(animationTime);
        _animator.SetFloat("InventoryTime", animationTime);
    }

    private void LateUpdate()
    {
        _dragAndDrop.OnLateUpdate();
    }

    public ItemStack GetHotbarItemAtSlotIndex(int index) => _hotbarSection.GetItemAtSlotIndex(index);

    public bool HasItemOnHotbar(ItemIdentity identity) => _hotbarSection.HasItem(identity);


    public void TakeInAsManyAsFit(ItemStack item)
    {
        if (item.amount <= 0)
        {
            throw new System.Exception("Trying to take in items from a stack whose amount is "
                + item.amount + ", item stack: identity: " + item.identity);
        }

        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i].TakeIntoExistingStacks(item);
        }
        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i].TakeIntoNewStacks(item);
        }

        if (item.amount < 0)
        {
            throw new System.Exception("took too many. item.amount: " + item.amount + ", item stack: identity: " + item.identity);
        }
    }
       
    public bool TrySubtractItemAmount(ItemIdentity identity, int amount)
    {
        int totalAmount = 0;
        for (int i = 0; i < _sections.Length; i++)
        {
            totalAmount += _sections[i].CountTotalAmount(identity);
        }
        if (totalAmount < amount)
        {
            return false;
        }

        for (int i = 0; i < _sections.Length; i++)
        {
            _sections[i].SubtractAmount(identity, ref amount);
        }
        if (amount != 0)
        {
            throw new System.Exception("amount should be 0 here but is " + amount);
        }

        return true;
    }

    public bool HasRoomForItem(ItemIdentity identity, int amount)
    {
        int countCanAdd = 0;
        for (int i = 0; i < _sections.Length; i++)
        {
            countCanAdd += _sections[i].CountCanAdd(identity);
        }
        return amount <= countCanAdd;
    }

    public static bool TryMoveItemBetweenSlots(InventorySlot from, InventorySlot to)
    {
        if (from == null)
        {
            throw new System.Exception("trying to move item from a null slot");
        }

        bool succeed = from != to && !InventoryInfoGetter.UnmatchedSortTypes(from.SortType, to.SortType);

        if (succeed)
        {
            ItemStack temp = from._itemStack;
            from._itemStack = to._itemStack;
            to._itemStack = temp;
            from.OnItemStackChanged();
            to.OnItemStackChanged();
        }

        return succeed;
    }




    private void OnBackpack(InputAction.CallbackContext context)
    {
        if (IgnoreInput.AnyReasons)
        {
            return;
        }

        _isInBackpackMode = !_isInBackpackMode;

        CursorMode.ReasonsForUnlockedCursor.ChangeReason("inventory open", _isInBackpackMode);
        InputIgnoring.ChangeReasonToIgnoreInputsForMovementThings("inventory open", _isInBackpackMode);

        if (_isInBackpackMode)
        {
            _dragAndDrop.EnableInput();
        }
        else
        {
            _dragAndDrop.DisableInputAndStop();
        }
    }

}