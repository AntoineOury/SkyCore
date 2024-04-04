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

    public Feeding Feeding { get; private set; }


    private void Awake()
    {
        _jellyParams = GetComponent<Parameters>();
        Feeding = GetComponent<Feeding>();
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
                    Feeding.TryFeedJelly(food.SaturationValue);
                    return true;
                }
                return false;

        default:
            return false;
        }
    }
}
