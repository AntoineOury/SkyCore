using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Watching : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private JellyStateMachine _stateMachine;
    [SerializeField]
    private GameEventScriptableObject _updateWatching;
    [SerializeField]
    private GameEventScriptableObject _exitWatching;
    [SerializeField, Range(0, 10), Tooltip("The distance the jelly watches the subject from")]
    private int _watchDistance;
    [SerializeField, Range(1, 180), Tooltip("The speed at which the jelly rotates to face the player (in degrees per second).")]
    private float _turnToFacePlayerSpeed = 60f;


    private Transform _player;
    private GameEventResponses _gameEventResponses = new();

    private void Awake()
    {
        _player = Player.Motion.PlayerMovement.Instance.transform;

        _gameEventResponses.SetSelectiveResponses(_stateMachine
            , (_updateWatching, UpdateWatching)
            , (_exitWatching, ExitWatching)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    private void UpdateWatching()
    {
        if (_agent.enabled)
        {
            _agent.stoppingDistance = _watchDistance;
            _agent.SetDestination(_player.position);
        }

        TurnToFacePoint(_player.position);
    }

    private void TurnToFacePoint(Vector3 position)
    {
        Quaternion q = Quaternion.LookRotation(position - _agent.transform.position);
        Vector3 eulerAngles = q.eulerAngles;
        
        // Clear x and z rotations to 0 so the jelly doesn't tilt down and look at the player's feet.        
        eulerAngles.x = eulerAngles.z = 0f;

        // Calculate the rotation between the Jelly's direction and the direction the player is in.
        eulerAngles.y = Mathf.Clamp(eulerAngles.y - _agent.transform.rotation.eulerAngles.y, 
                                    -_turnToFacePlayerSpeed,
                                    _turnToFacePlayerSpeed) * Time.deltaTime;

        eulerAngles.y += _agent.transform.rotation.eulerAngles.y;
        q.eulerAngles = eulerAngles;

        _agent.transform.rotation = q;
    }

    private void ExitWatching()
    {
        _agent.ResetPath();
    }
}
