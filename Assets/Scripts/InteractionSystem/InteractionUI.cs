using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I think this code is just for the prompt "E" ui. So maybe this can be controlled by the PlayerInteraction script instead,
// if that's the only thing which uses the InteractionUI prefab.

// Should probably rename this and its prefab to InteractionPrompt or something.
// Probably shouldn't activate/deactivate the canvas gameobject via game event scriptable objects, because the code which needs
// the prompt to be shown shouldn't be decoupled from the prompt.


public class InteractionUI : MonoBehaviour
{
    private static InteractionUI _instance;
    public static InteractionUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindWithTag("InteractionUI").GetComponent<InteractionUI>();
            }
            return _instance;
        }
    }

    public Reasons DontInteract { get; private set; }

    private void Awake()
    {
        DontInteract = new Reasons((anyReasons) => gameObject.SetActive(!anyReasons));
    }
}
