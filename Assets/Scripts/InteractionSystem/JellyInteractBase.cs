using Jellies;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class JellyInteractBase : InteractableWithUIMode
{
    [SerializeField] 
    private Slider _satiationSlider;
    [SerializeField] 
    private TextMeshProUGUI _satiationText;
    [SerializeField]
    private TextMeshProUGUI _satiationText2;

    private Image _satiationSliderFillArea;

    private Parameters _jellyParams;

    private FoodPreferences _foodPreferences;

    public Feeding Feeding { get; private set; }


    private void Awake()
    {
        _satiationSliderFillArea = _satiationSlider.transform.Find("Fill").GetComponent<Image>();
        _jellyParams = GetComponent<Parameters>();
        Feeding = GetComponent<Feeding>();
        _foodPreferences = GetComponent<FoodPreferences>();
    }

    private void Update()
    {
        if (IsInteracting)
        {
            _satiationSlider.maxValue = _jellyParams.MaxSatiation;
            _satiationSlider.value = _jellyParams.Satiation;
            _satiationSliderFillArea.color = GetSatiationBarColor();
            _satiationText.SetText($"Hunger: {_jellyParams.Satiation}/{_jellyParams.MaxSatiation}");
            _satiationText2.SetText(_jellyParams.HungerLevelName());
        }
    }

    public override bool InventoryInteraction(ItemIdentity item)
    {
        switch (item) {
            case FoodItemIdentity food:
                if(_jellyParams.CanEat && _jellyParams.Satiation < _jellyParams.MaxSatiation)
                {
                    int adjustedSatiation = _foodPreferences.GetAdjustedSatiation(food);
                    if(adjustedSatiation < food.SatiationValue)
                    {
                        // Could replace with a way to decrease how much the Jelly likes the player 
                        Debug.Log("The Jelly did not like that food");
                    } else
                    {
                        // Could replace with a way to increase how much the Jelly likes the player
                        Debug.Log("The jelly loved that food");
                    }
                    return Feeding.TryFeedJelly(adjustedSatiation);
                }
                return false;

        default:
            return false;
        }
    }


    private Color GetSatiationBarColor()
    {
        switch (_jellyParams.HungerState)
        {
            case HungerStates.Hungry:
                return Color.Lerp(Color.red,
                                  new Color32(255, 128, 0, 255), // orange
                                  _jellyParams.Satiation / _jellyParams.HungerStateSlightlyFedThreshold);
                return Color.red;

            case HungerStates.SlightlyFed:
                return Color.Lerp(new Color32(255, 128, 0, 255), // orange
                                  Color.yellow,
                                  (_jellyParams.Satiation - _jellyParams.HungerStateSlightlyFedThreshold) / (_jellyParams.MaxSatiation - _jellyParams.HungerStateSlightlyFedThreshold));

            case HungerStates.Satisfied:
                return Color.Lerp(Color.yellow,
                                  Color.green,
                                  (_jellyParams.Satiation - _jellyParams.HungerStateSatisfiedThreshold) / (_jellyParams.MaxSatiation - _jellyParams.HungerStateSatisfiedThreshold));

            case HungerStates.Full:
                return Color.green;


            default:
                throw new System.Exception("Invalid hunger state!");
        }
    }


}
