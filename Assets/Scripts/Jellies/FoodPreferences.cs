using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jellies
{
    /// <summary>
    /// 
    /// </summary>
    public class FoodPreferences : MonoBehaviour
    {
        [SerializeField, Tooltip("The range preferences can have")]
        private int preferenceRange;

        [field: SerializeField, Tooltip("How sour is the food")]
        public int Sour { get; private set; }
        [field: SerializeField, Tooltip("How sweet is the food")]
        public int Sweet { get; private set; }
        [field: SerializeField, Tooltip("How savory is the food?")]
        public int Savory { get; private set; }
        [field: SerializeField, Tooltip("How salty is the food?")]
        public int Salty { get; private set; }
        [field: SerializeField, Tooltip("How bitter is the food?")]
        public int Bitter { get; private set; }
        [field: SerializeField, Tooltip("How spicy is the food?")]
        public int Spicy { get; private set; }

        private void Start()
        {
            List<int> preferences = new List<int>();
            preferences.Add(Sour = UnityEngine.Random.Range(1, preferenceRange));
            preferences.Add(Sweet = UnityEngine.Random.Range(1, preferenceRange));
            preferences.Add(Savory = UnityEngine.Random.Range(1, preferenceRange));
            preferences.Add(Salty = UnityEngine.Random.Range(1, preferenceRange));
            preferences.Add(Bitter = UnityEngine.Random.Range(1, preferenceRange));
            preferences.Add(Spicy = UnityEngine.Random.Range(1, preferenceRange));
            preferences.Sort();
            int multiplier = -1;
            for (int i = 0; i < 3; i++)
            {
                int preference = UnityEngine.Random.Range(1, 6);
                switch (preference)
                {
                    case 1: Sour *= multiplier; break;
                    case 2: Sweet *= multiplier; break;
                    case 3: Savory *= multiplier; break;
                    case 4: Salty *= multiplier; break;
                    case 5: Bitter *= multiplier; break;
                    case 6: Spicy *= multiplier; break;
                }
            }
        }

        public int GetAdjustedSatiation(FoodItemIdentity food)
        {
            int sum = 0;
            sum += Sour >= 0 ? Sour * food.Sour : Sour;
            sum += Sweet >= 0 ? Sweet * food.Sweet : Sweet;
            sum += Savory >= 0 ? Savory * food.Savory : Savory;
            sum += Salty >= 0 ? Salty * food.Salty : Salty;
            sum += Bitter >= 0 ? Bitter * food.Bitter : Bitter;
            sum += Spicy >= 0 ? Spicy * food.Spicy : Spicy;

            return sum;
        }

    }
}