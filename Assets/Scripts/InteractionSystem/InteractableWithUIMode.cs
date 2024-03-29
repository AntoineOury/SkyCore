using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableWithUIMode : Interactable
{
    [SerializeField]
    private float _reactivateTime = 1.25f;

    [SerializeField]
    private GameObject _virtualCamera;

    private bool _interacting;
    public bool IsInteracting
    {
        get => _interacting;
        protected set
        {
            if (value != _interacting)
            {
                _interacting = value;

                // Only one can interact at a time, so can do it like this.
                CurrentlyInteracting = value ? this : null;
            }
        }
    }
    public static InteractableWithUIMode CurrentlyInteracting { get; private set; }
    public static bool AnyInteracting => CurrentlyInteracting != null;

    public override void OnInteractAction()
    {
        SetInteract(true);
    }

    public void InteractStop()
    {
        SetInteract(false);
    }

    private void SetInteract(bool interact)
    {
        IsInteracting = interact;

        _virtualCamera.SetActive(interact);

        CursorMode.ReasonsForUnlockedCursor.ChangeReason("interacting", interact);

        if (interact)
        {
            SetUIActive(false);
        }
        else
        {
            Invoke(nameof(ReactivateUI), _reactivateTime);
        }
    }

    private void ReactivateUI()
    {
        SetUIActive(true);
    }

    private void SetUIActive(bool active)
    {
        InputIgnoring.ChangeReasonToIgnoreInputsForMovementAndInteractionThings("interacting", !active);
    }

}
