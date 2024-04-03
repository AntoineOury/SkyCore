using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FoodItemIdentity", menuName = "ItemIdentity/FoodItemIdentity")]
public class FoodItemIdentity : ItemIdentity
{
    [field: SerializeField]
    public bool IsIngredientInCauldron { get; private set; }
    [field: SerializeField, Tooltip("Saturation value to be added to jelly after feeding")]
    public int SaturationValue { get; private set; }
    [field: SerializeField, Tooltip("How sour is the food?")]
    public int Sour { get; private set; }
    [field: SerializeField, Tooltip("How sweet is the food?")]
    public int Sweet { get; private set; }
    [field: SerializeField, Tooltip("How savory is the food?")]
    public int Savory { get; private set; }
    [field: SerializeField, Tooltip("How salty is the food?")]
    public int Salty { get; private set; }
    [field: SerializeField, Tooltip("How bitter is the food?")]
    public int Bitter { get; private set; }
    [field: SerializeField, Tooltip("How spicy is the food?")]
    public int Spicy { get; private set; }
}
