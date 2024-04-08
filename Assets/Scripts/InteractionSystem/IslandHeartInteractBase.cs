using UnityEngine;
using Player.View;
using Player.Motion;
using TMPro;
using UnityEngine.UI;

public class IslandHeartInteractBase : InteractableWithUIMode
{

    private IslandHeartLeveling heart;

    private void Awake()
    {
        heart = gameObject.GetComponent<IslandHeartLeveling>();

    }

    private void Update()
    {
        
    }

    public override bool InventoryInteraction(ItemIdentity item)
    {
        if (item == null)
        {
            Debug.Log("You do not have an item selected");
            return false;
        }
        switch (item)
        {
            case GeneratorItemIdentity generator:
                GameObject generatorSpawned = Instantiate(item.ItemPrefab);
                return heart.PlaceGenerator(generatorSpawned, true);

            case JellyDewItemIdentity jellyDew:
                heart.FeedIslandHeart(1);
                return true;

            default:
                return false;
        }
    }
}
