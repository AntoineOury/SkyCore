using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : Interactable
{
    [SerializeField, Tooltip("How long the bush takes to regrow berries")]
    private int _cooldown;
    
    [SerializeField]
    private ItemIdentity _foodItemIdentity;
    private bool _canBeHarvested = false;
    [SerializeField, Tooltip("The number of berries to be harvested")]
    private int _numOfBerries;

    [SerializeField, Tooltip("The visual indicator that the bush is ready to harvest")]
    private GameObject _indicator;

    void Start()
    {
        SpawnBerry();

    }
    private void Update()
    {
        if (_canBeHarvested && !_indicator.activeSelf)
        {
            _indicator.SetActive(true);
        }
        else if (!_canBeHarvested && _indicator.activeSelf)
        {
            _indicator.SetActive(false);
        }
    }

    private void SpawnBerry()
    {
        _canBeHarvested = true;
        _indicator.SetActive(true);
    }

    public override void OnInteractAction()
    {
        if (_canBeHarvested)
        {
            ItemStack item = new ItemStack(_foodItemIdentity, _numOfBerries);
            Inventory.Instance.TakeInAsManyAsFit(item);

            _canBeHarvested = false;
            _indicator.SetActive(false);
			
            StartCoroutine(BushCooldown());
        }
    }

    private IEnumerator BushCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        SpawnBerry();
    }
}
