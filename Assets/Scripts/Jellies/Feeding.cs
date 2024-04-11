using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Player;

namespace Jellies
{
    public enum HungerStates
    {
        Hungry = 0,
        SlightlyFed,
        Satisfied,
        Full
    }



    /// <summary>
    /// Currently a stub. Intended functionality:
    /// Handling of Jelly feeding mechanic, checking if the player can feed the Jelly.
    /// If the player feeds the Jelly, then this handles the Jelly's reactions and output objects
    /// </summary>
    public class Feeding : MonoBehaviour
    {
        
        public HungerStates HungerState { get; private set; }


        private Parameters _parameters;

        private DewInstantiate _dew;

        private SlimeExperience _slimeExp;



        private void Awake()
        {
            _parameters = GetComponent<Parameters>();
            _dew = GetComponent<DewInstantiate>();
            _slimeExp = GetComponent<SlimeExperience>();
        }

        /// <summary>
        /// Feeds jelly by increasing it's food saturation.
        /// </summary>
        /// <param name="satiationAmount">Amount of food that is being fed to the jelly.</param>
        public bool TryFeedJelly(float satiationAmount)
        {
            if (_parameters.Satiation >= _parameters.MaxSatiation)
            {
                if (_parameters.Satiation > _parameters.MaxSatiation)
                {
                    throw new Exception("In Feeding, _parameters.Satiation >= _parameters.MaxSatiation: "
                        + _parameters.Satiation + " " + _parameters.MaxSatiation);
                }
                return false;
            }
            _parameters.IncreaseSatiation(satiationAmount);

            int index = Math.Min(_parameters.NumOfDewSpawnedAtLevel.Length - 1, _slimeExp.LevelNum - 1);
            _dew.DewSpawn(_parameters.NumOfDewSpawnedAtLevel[index]);

            return true;
        }

        
    }
}


