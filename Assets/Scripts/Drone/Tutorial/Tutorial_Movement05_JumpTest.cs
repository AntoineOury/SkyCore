using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;
using Player.View;
using Player.Motion;

public class Tutorial_Movement05_JumpTest : MonoBehaviour
{
    [Header("Tutorial Options")]
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private DroneMovement _movement;

    [Header("Tutorial Options")]
    [Tooltip("This is how long (in seconds) the drone will display the jump tutorial complete image.")]
    [SerializeField, Min(0f)]
    private float _jumpTestSuccessDelay = 2.0f;
    [Tooltip("This is how long (in seconds) there is after the success graphic appears")]
    [SerializeField, Min(0f)]
    private float _droneTutorialCompleteDisplayDuration = 3.0f;

    [Header("Drone Pictogram")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;
    [SerializeField]
    private Sprite _sprite_DroneJumpControls;
    [SerializeField]
    private Sprite _sprite_DroneSuccess;
    [SerializeField]
    private Sprite _sprite_DroneTutorialComplete;


    [Header("Finite State Machine Parameters")]
    [Tooltip("This parameter tracks what step the tutorial is currently in.")]
    [SerializeField]
    private FSMParameter _triggerForNextPartOfTutorial;

    [Header("Tutorial State Machine Events - Stage 05 - Jump Test")]
    [SerializeField]
    private GameEventScriptableObject _Enter;
    [SerializeField]
    private GameEventScriptableObject _Update;
    [SerializeField]
    private GameEventScriptableObject _Exit;



    private FiniteStateMachineInstance _stateMachineInstance;

    private bool _jumpTestPassed;

    private GameEventResponses _gameEventResponses = new();

    private bool _waitingForTutorialDronePictureTimeToEnd;



    private void Awake()
    {
        // Get the PlayerInputComponent.
        _stateMachineInstance = _drone.GetActionStateMachineInstance();

        _gameEventResponses.SetResponses(
            // Player controls events
            (_Enter, EnterTutorial),
            (_Update, UpdateTutorial),
            (_Exit, ExitTutorial)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();


    private void EnterTutorial()
    {
        PlayerMovement.Instance.IgnoreJumpInput.RemoveReason("movement tutorial");

        _waitingForTutorialDronePictureTimeToEnd = false;
        _jumpTestPassed = false;

        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneJumpControls);
        CameraSystem.SwitchToTutorialCamera();
    }

    private void UpdateTutorial()
    {
        if (_jumpTestPassed && !_waitingForTutorialDronePictureTimeToEnd)
        {
            StartCoroutine(WaitForTutorialDonePictureDisplayTimeToEnd());
        }
        
        if (PlayerMovement.Instance.GetJumpInput())
        {
            _jumpTestPassed = true;
        }
    }

    private void ExitTutorial()
    {
        // Switch back to the normal player camera.
        CameraSystem.SwitchToFirstPersonCamera();
        PlayerMovement.Instance.IgnoreJumpInput.AddReason("movement tutorial");

    }
   
    private IEnumerator WaitForTutorialDonePictureDisplayTimeToEnd()
    {
        _waitingForTutorialDronePictureTimeToEnd = true;

        // Show the success image.
        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneSuccess);
        yield return new WaitForSeconds(_jumpTestSuccessDelay);


        // Show the tutorial complete image.
        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneTutorialComplete);
        yield return new WaitForSeconds(_droneTutorialCompleteDisplayDuration);


        _waitingForTutorialDronePictureTimeToEnd = false;

        _stateMachineInstance.SetTrigger(_triggerForNextPartOfTutorial);

    }


}
