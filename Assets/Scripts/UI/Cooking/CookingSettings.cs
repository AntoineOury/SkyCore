using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cooking Settings", menuName = "Misc/Cooking Settings")]
public class CookingSettings : ScriptableObject
{
    [field: SerializeField, Tooltip("How long it takes to cook something in the cauldron.")]
    public float CookingTime { get; private set; }

    [field: SerializeField, Tooltip("When the player puts items in the cauldron without a corresponding recipe, it produces this.")]
    public ItemIdentity ProductWhenIngredientsMatchNoRecipe { get; private set; }

    [field: SerializeField, Tooltip("Recipes for the cauldron.")]
    public Recipe[] Recipes { get; private set; }

    [System.Serializable]
    public class Recipe
    {
        [field: SerializeField]
        public FoodItemIdentity IngredientA { get; private set; }

        [field: SerializeField]
        public FoodItemIdentity IngredientB { get; private set; }

        [field: SerializeField]
        public FoodItemIdentity Product { get; private set; }
    }

    public ItemIdentity DetermineProduct(ItemIdentity a, ItemIdentity b)
    {
        for (int i = 0; i < Recipes.Length; i++)
        {
            Recipe recipe = Recipes[i];
            if (recipe.IngredientA == a && recipe.IngredientB == b
                || recipe.IngredientA == b && recipe.IngredientB == a)
            {
                return recipe.Product;
            }
        }
        return ProductWhenIngredientsMatchNoRecipe;
    }
}
