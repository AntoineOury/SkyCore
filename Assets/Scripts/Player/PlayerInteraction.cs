using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class managing detection of interactable objects by the player
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private float _interactionDistance = 3f;

    [SerializeField]
    private LayerMask _interactableLayer;

    private Interactable _currentInteractable;
    private Camera _mainCamera;


    private static PlayerInteraction _instance;
    public static PlayerInteraction Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindWithTag("Player").GetComponent<PlayerInteraction>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _mainCamera = Camera.main;
        _instance = this;
        GetComponent<PlayerInput>().actions.FindAction("Interact").started += OnInteractAction;
    }

    private void Update()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (!Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactableLayer, QueryTriggerInteraction.Ignore))
        {
            if (_currentInteractable != null)
            {
                InteractionPrompt.Instance.HidePrompt();
                _currentInteractable = null;
            }
            return;
        }
        
        if (_currentInteractable == null)
        {
            Interactable hitInteractable = hit.collider.GetComponent<Interactable>();

            if (hitInteractable != null)
            {
                _currentInteractable = hitInteractable;
                InteractionPrompt.Instance.ShowPrompt();
            }
        }
    }

    private void OnInteractAction(InputAction.CallbackContext context)
    {
        if (InteractionPrompt.Instance.DontInteract.AnyReasons)
        {
            return;
        }

        if (_currentInteractable != null)
        {
            _currentInteractable.OnInteractAction();
        }
    }
}