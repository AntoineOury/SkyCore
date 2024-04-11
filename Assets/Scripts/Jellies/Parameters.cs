using System;
using System.Collections;
using UnityEngine;

namespace Jellies
{
    /// <summary>
    /// Holds what are the different type of jellies are.
    /// </summary>
    [Tooltip("Holds what are the different type of jellies are.")]
    [Serializable]
    public enum JellyType
    {
        Base, Tumble, MonsterTumbleEvolutionTest
    }

    /// <summary>
    /// Handles updating and storing Jelly information. 
    /// It handles the type of Jelly a Jelly is, ensures Jelly Satiation levels are updated, with the Food Saturation decreasing at regular intervals, 
    /// and updates the Jelly experience system whenever the Jelly is fed.
    /// </summary>
    public class Parameters : MonoBehaviour
    {
        private float _timeSinceLastFed;



        /// <summary>
        /// Returns true if the jelly is not already eating a piece of food, or false if it is.
        /// </summary>
        public bool CanEat { get { return _timeSinceLastFed >= _eatingTime; } }

        /// <summary>
        /// What type of jelly is it.
        /// </summary>
        [Tooltip("What type of jelly is it.")]
        [field: SerializeField]
        public JellyType jellyType
        {
            get;
            private set;
        }

        /// <summary>
        /// The number of dew spawned per interaction.
        /// </summary>
        [Tooltip("The number of dew spawned per interaction. Index 0 is assumed to be level 1, index 1 level 2 and so on.")]
        [field: SerializeField]
        public int[] NumOfDewSpawnedAtLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// The required experience per level
        /// </summary>
        [Tooltip("The required experience per level. Index 0 is assumed to be level 1, index 1 level 2 and so on.")]
        [field: SerializeField]
        public float[] RequiredExpForNextLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// How well fed is the jelly. Decreases over time.
        /// </summary>
        [Tooltip("How well fed is the jelly. Decreases over time.")]
        public float Satiation
        {
            get;
            private set;
        }

        /// <summary>
        /// How full can the jelly get. 
        /// </summary>
        [Tooltip("How full can the jelly get.")]
        public float MaxSatiation
        {
            get { return _hungerStateFullThreshold; }
        }


        /// <summary>
        /// How often to decrease the hunger.
        /// </summary>
        [Tooltip("How often (in seconds) to decrease the hunger.")]
        [SerializeField]
        private float _satiationDecreaseInterval;

        /// <summary>
        /// How much to decrease the Satiation by.
        /// </summary>
        [Tooltip("How much to decrease the hunger by each time SatiationDecreaseInterval seconds have elapsed.")]
        [SerializeField]
        private float _satiationDecreaseAmount;

        [Tooltip("How long it takes (in seconds) for the jelly to eat a piece of food.")]
        [SerializeField]
        private float _eatingTime;

       
        [Tooltip("How much food must be in the jelly's stomach for it to enter the slightly fed hunger state.")]
        [SerializeField]
        private float _hungerStateSlightlyFedThreshold = 33;

        [Tooltip("How much food must be in the jelly's stomach for it to enter the satisfied hunger state.")]
        [SerializeField]
        private float _hungerStateSatisfiedThreshold = 66;

        [Tooltip("How much food must be in the jelly's stomach for it to enter the full hunger state.")]
        [SerializeField]
        private float _hungerStateFullThreshold = 100;


        public float HungerStateSlightlyFedThreshold { get { return _hungerStateSlightlyFedThreshold; } }
        public float HungerStateSatisfiedThreshold { get { return _hungerStateSatisfiedThreshold; } }
        public float HungerStateFullThreshold { get { return _hungerStateFullThreshold; } }


        public HungerStates HungerState
        {
            get
            {
                if (Satiation < 0 || Satiation > _hungerStateFullThreshold)
                    throw new Exception("Invalid hunger value: " + Satiation);


                else if (Satiation < _hungerStateSlightlyFedThreshold)
                    return HungerStates.Hungry;
                else if (Satiation < _hungerStateSatisfiedThreshold)
                    return HungerStates.SlightlyFed;
                else if (Satiation < _hungerStateFullThreshold)
                    return HungerStates.Satisfied;
                else if (Satiation == _hungerStateFullThreshold)
                    return HungerStates.Full;                
                else
                    throw new Exception("Invalid hunger value: " + Satiation);
            }
        }

        public string HungerLevelName()
        {
            string name = Enum.GetName(typeof(HungerStates), HungerState);

            // Insert a space before each uppercase letter except the first one.
            for (int i = name.Length - 1; i >= 0; i--)
            {
                if (char.IsUpper(name[i]) && i > 0)
                    name.Insert(i - 1, " ");
            }

            return name;
        }

        /// <summary>
        /// Xp Controller for the slime.
        /// TODO: Replace this with a Event
        /// </summary>
        [Tooltip("Xp Controller for the slime.")]
        [SerializeField]
        private SlimeExperience _slimeXp;

        /// <summary>
        /// Returns the type of Jelly.
        /// </summary>
        public JellyType TypeOfThisJelly()
        {
            return jellyType;
        }

        /// <summary>
        /// Feed the jelly a given amount.
        /// </summary>
        public void IncreaseSatiation(float amount)
        {
            Satiation = Mathf.Clamp(Satiation + amount, 0, MaxSatiation);
            
            // TODO: Come back and make this more dynamic and replace with a event.
            if (_slimeXp == null && transform.GetComponentInChildren<SlimeExperience>())
            {
                _slimeXp = transform.GetComponentInChildren<SlimeExperience>();
            }
            else if (_slimeXp != null)
            {
                _slimeXp.AddEXP(10, "Red Berries");
            }

            // Reset the eating timer.
            _timeSinceLastFed = 0f;
        }

        /// <summary>
        /// Jelly loses some food Saturation when going to the bathroom.
        /// </summary>
        public void DecreaseSatiation(float amount)
        {
            // The Mathf.Abs() call here is necessary, because subtracting a negative number is the same as adding a positive, which is not what we should do here.
            Satiation = Mathf.Clamp(Satiation - Mathf.Abs(amount), 0, MaxSatiation);
        }

        /// <summary>
        /// Set how full is the jelly's belly.
        /// Used for Unit Testing and can be removed if necessary
        /// </summary>
        public void SetSatiation(float amount)
        {
            // If we increased satiation, reset the eating timer.
            if (amount > Satiation)
                _timeSinceLastFed = 0f;

            Satiation = Mathf.Clamp(Satiation + amount, 0, MaxSatiation);
        }

        private void Awake()
        {
            Satiation = MaxSatiation;
        }

        /// <summary>
        /// Call on GameObject creation
        /// </summary>
        private void Start()
        {
            StartCoroutine(Digest());
        }

        private void Update()
        {
            // Increment the time since last given food timer.
            _timeSinceLastFed += Time.deltaTime;
        }

        /// <summary>
        /// A function that decreases the hunger over time.
        /// </summary>
        /// <returns>Next time to run function</returns>
        IEnumerator Digest()
        {
            DecreaseSatiation(_satiationDecreaseAmount);
            yield return new WaitForSeconds(_satiationDecreaseInterval);
            StartCoroutine(Digest());
        }
    }
}