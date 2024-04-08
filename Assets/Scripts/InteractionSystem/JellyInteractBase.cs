using Jellies;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JellyInteractBase : InteractableWithUIMode
{
    [SerializeField] 
    private Slider _saturationSlider;
    [SerializeField] 
    private TextMeshProUGUI _saturationText;

    private Parameters _jellyParams;

    private FoodPreferences _foodPreferences;

    public Feeding Feeding { get; private set; }


    private void Awake()
    {
        _jellyParams = GetComponent<Parameters>();
        Feeding = GetComponent<Feeding>();
        _foodPreferences = GetComponent<FoodPreferences>();
    }

    private void Update()
    {
        if (IsInteracting)
        {
            _saturationSlider.value = _jellyParams.FoodSaturation;
            _saturationText.SetText($"Saturation: {_jellyParams.FoodSaturation}");
        }
    }

    public override bool InventoryInteraction(ItemIdentity item)
    {
        switch (item) {
            case FoodItemIdentity food:
                if(_jellyParams.FoodSaturation < _jellyParams.MaxFoodSaturation)
                {
                    int adjustedSaturation = _foodPreferences.GetAdjustedSaturation(food);
                    if(adjustedSaturation < food.SaturationValue)
                    {
                        // Could replace with a way to decrease how much the Jelly likes the player 
                        Debug.Log("The Jelly did not like that food");
                    } else
                    {
                        // Could replace with a way to increase how much the Jelly likes the player
                        Debug.Log("The jelly loved that food");
                    }
                    return Feeding.TryFeedJelly(adjustedSaturation);
                }
                return false;

        default:
            return false;
        }
    }
}
