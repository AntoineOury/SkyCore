using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private GameObject _promptToggle;

    

    private static InteractionPrompt _instance;
    public static InteractionPrompt Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindWithTag("InteractionUI").GetComponent<InteractionPrompt>();
            }
            return _instance;
        }
    }

    public Reasons DontInteract { get; private set; }

    private void Awake()
    {
        DontInteract = new Reasons((anyReasons) => gameObject.SetActive(!anyReasons));
    }

    public void ShowPrompt() => _promptToggle.SetActive(true);
    public void HidePrompt() => _promptToggle.SetActive(false);
}
