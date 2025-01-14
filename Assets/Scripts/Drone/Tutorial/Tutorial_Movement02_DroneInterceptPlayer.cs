using Cinemachine;
using FiniteStateMachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial_Movement02_DroneInterceptPlayer : MonoBehaviour
{
    [Header("Tutorial Options")]
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private DroneMovement _droneMovement;
    [SerializeField]
    private DroneScanning _droneScanning;


    [Header("Tutorial Settings")]
    [SerializeField]
    private float _droneDistanceToStopInFrontOfPlayer;
    [Tooltip("After the cutscene has ended, this is the delay between when the drone starts moving toward the player and when the camera starts to turn to look at the drone.")]
    [SerializeField, Min(0)]
    private float _cameraLookAtDroneDelay = 1f;
    [Tooltip("This sets how long it takes (in seconds) the drone to perform medical assistance when it intercepts the player right after the cutscene plays.")]
    [SerializeField, Min(0)]
    private float _medicalAssistanceDuration = 3f;
    [Tooltip("This sets how long (in seconds) the drone's scan success visual is displayed for.")]
    [SerializeField, Min(0)]
    private float _medicalAssistanceCompleteDelay = 1.5f;

    [Header("Drone Pictogram")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;
    [SerializeField]
    private Sprite _sprite_DroneMedicalAssistance;

    [Header("Action Finite State Machine Parameters")]
    [SerializeField]
    private FSMParameter _triggerForNextPartOfTutorial;

    [Header("Movement Finite State Machine Parameters")]
    [SerializeField]
    private FSMParameter _interceptPlayer;

    [Header("Tutorial State Machine Events - Stage 02 - Drone Intercepts Player")]
    [SerializeField]
    private GameEventScriptableObject _Enter;
    [SerializeField]
    private GameEventScriptableObject _Update;
    [SerializeField]
    private GameEventScriptableObject _Exit;

    [Header("Events in the Movement State Machine")]
    [SerializeField]
    private GameEventScriptableObject _UpdateInterceptPlayer;



    private FiniteStateMachineInstance _actionStateMachineInstance;
    private FiniteStateMachineInstance _movementStateMachineInstance;

    private Transform _player;


    private GameEventResponses _gameEventResponses = new();

    private bool _startedMedicalAssistance;
    private bool _medicalAssistanceComplete;

    private bool _finishedInterceptingPlayer;



    private void Awake()
    {
        _actionStateMachineInstance = _drone.GetActionStateMachineInstance();
        _movementStateMachineInstance = _drone.GetMovementStateMachineInstance();

        _player = Player.Motion.PlayerMovement.Instance.transform;

        _gameEventResponses.SetResponses(
            (_Enter, EnterTutorial),
            (_Update, UpdateTutorial),
            (_Exit, ExitTutorial),
            (_UpdateInterceptPlayer, UpdateInterceptPlayer)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();


    private void EnterTutorial()
    {
        _movementStateMachineInstance.SetBool(_interceptPlayer, true);

        _medicalAssistanceComplete = false;
        _startedMedicalAssistance = false;

        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneMedicalAssistance);

        StartCoroutine(WaitToFocusCamera());
    }

    private void UpdateInterceptPlayer()
    {
        // This runs from FixedUpdate
        _finishedInterceptingPlayer = _droneMovement.MoveDroneInFrontOfPlayer(_droneDistanceToStopInFrontOfPlayer, true);
    }

    private void UpdateTutorial()
    {
        // This currently runs from Update

        // The drone moves into view until it is right in front of the player.
        if (_finishedInterceptingPlayer && !_startedMedicalAssistance)
        {
            // The drone has reached the player, so display the health image.
            _pictogramBehaviour.SetImageActive(true);

            _startedMedicalAssistance = true;

            // Show the drone's scanning visual.
            _droneScanning.ShowScanningVisual(_player.transform.position);
            StartCoroutine(WaitForMedicalAssistanceToFinish());
        }


        if (_medicalAssistanceComplete)
        {
            // Trigger transition to next part of the tutorial.
            _actionStateMachineInstance.SetTrigger(_triggerForNextPartOfTutorial);
        }

    }

    private IEnumerator WaitForMedicalAssistanceToFinish()
    {
        float elapsedTime = 0f;
        while (elapsedTime <= _medicalAssistanceDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // Hide the drone's scanning visual, and show the success visual.
        _droneScanning.HideScanningVisual();
        _droneScanning.ShowScanningSuccessVisual(_player.transform.position);
        
        // Hide the drone's scanning success visual after the specified delay.
        yield return new WaitForSeconds(_medicalAssistanceCompleteDelay);
        _droneScanning.HideScanningSuccessVisual();


        _medicalAssistanceComplete = true;
    }

    private IEnumerator WaitToFocusCamera()
    {
        float elapsedTime = 0f;
        while (elapsedTime <= _cameraLookAtDroneDelay)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // Switch to the tutorial camera.
        CameraSystem.SwitchToTutorialCamera();

        CameraSystem.TutorialCamera.LookAt = _drone.transform;
    }

    private void ExitTutorial()
    {
        _movementStateMachineInstance.SetBool(_interceptPlayer, false);

        CameraSystem.TutorialCamera.LookAt = null;       
    }

   
}
