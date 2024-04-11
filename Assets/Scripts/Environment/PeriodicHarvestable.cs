using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicHarvestable : Interactable
{
    [SerializeField]
    private GameObject _harvestableIndicator;
    [SerializeField]
    private ItemIdentity _itemIdentity;
    [SerializeField]
    private int _numberToHarvest;
    [SerializeField, Tooltip("How long it takes to become harvestable again.")]
    private float _cooldown;
    

    private ItemStack _itemStack;

    private float _lastHarvestTime;

    private bool _harvestable = true;
    private bool Harvestable
    {
        get => _harvestable;
        set
        {
            _harvestable = value;
            _harvestableIndicator.SetActive(value);
        }
    }


    private void Awake()
    {
        _itemStack = ItemStack.ProduceObject(_itemIdentity, _numberToHarvest);
    }

    private void OnDestroy()
    {
        _itemStack.ReturnToPool();
    }

    private void Update()
    {
        if (!Harvestable && Time.time > _lastHarvestTime + _cooldown)
        {
            Harvestable = true;
        }
    }

    public override void OnInteractAction()
    {
        if (!Harvestable)
        {
            return;
        }

        // harvest
        _itemStack.amount = _numberToHarvest;
        Inventory.Instance.TakeInAsManyAsFit(_itemStack);
        _lastHarvestTime = Time.time;
        Harvestable = false;
    }
}
