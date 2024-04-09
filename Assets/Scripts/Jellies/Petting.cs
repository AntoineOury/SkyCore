using Jellies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Player;

namespace Jellies
{
    public class Petting : MonoBehaviour
    {
        private Parameters _parameter;

        private DewInstantiate _dew;

        private SlimeExperience _slimeExp;

        private JellyInteractBase _jellyInteract;

        [SerializeField]
        private GameObject _pettingButton;

        private Animator _animator;

        [SerializeField, Tooltip("How often the player can pet the jelly")]
        private float _pettingCooldown;
        // Start is called before the first frame update
        private void Awake()
        {
            _parameter = GetComponent<Parameters>();
            _dew = GetComponent<DewInstantiate>();
            _slimeExp = GetComponent<SlimeExperience>();
            _jellyInteract = GetComponent<JellyInteractBase>();
            _animator = GetComponent<Animator>();
            _parameter.CanBePet = true;
            //_pettingButton = GameObject.Find("PetButton").GetComponent<Button>();
        }

        // Update is called once per frame
        void Update()
        {
            
            
        }
        public void PetJelly()
        {
            if (_parameter.CanBePet)
            {
                int index = Math.Min(_parameter.NumOfDewSpawnedAtLevel.Length - 1, _slimeExp.LevelNum - 1);
                _parameter.CanBePet = false;
                
                _jellyInteract.SpawnDewAndAddEXP(_parameter.NumOfDewSpawnedAtLevel[index], 10, "Petting");
                StartCoroutine(Delay());
            }
        }
        private IEnumerator Delay()
        {
            _animator.Play("Petting",0,0);
            _pettingButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(_pettingCooldown);
            _pettingButton.gameObject.SetActive(true);
            _parameter.CanBePet = true;
        }
    }
}

