using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputIgnoring 
{
    public static void ChangeReasonToIgnoreInputsForMovementAndInteractionThings(string reason, bool addDontRemove)
    {
        ChangeReasonToIgnoreInputsForMovementThings(reason, addDontRemove);
        InteractionPrompt.Instance.DontInteract.ChangeReason(reason, addDontRemove);
    }

    public static void ChangeReasonToIgnoreInputsForMovementThings(string reason, bool addDontRemove)
    {
        Player.View.FirstPersonView.Instance.IgnoreInput.ChangeReason(reason, addDontRemove);
        Player.Motion.PlayerMovement.Instance.IgnoreWASDInput.ChangeReason(reason, addDontRemove);
        Player.Motion.PlayerMovement.Instance.IgnoreJumpInput.ChangeReason(reason, addDontRemove);

    }
}
