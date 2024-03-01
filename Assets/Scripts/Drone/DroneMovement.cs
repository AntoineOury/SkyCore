using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is placeholder movement. The drone will pass through blocks.
// We can either make it looks like the drone is super sci-fi and can pass thru blocks
// or do something like navmesh so it flies around obstacles.
// We might need to do our own navigation implementation if Unity's navmesh doesn't
// support 3d. Or look into other options.

public class DroneMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField, Tooltip("How fast the drone moves.")]
    private float _movementSpeed = 3f;
    [SerializeField, Tooltip("How fast the drone rotates towards something, in degrees/sec")]
    private float _turnSpeed = 300f;
    [SerializeField, Tooltip("How fast the drone spins while idling next to the player, in degrees/sec")]
    private float _idleSpinSpeed = 30f;
    [SerializeField, Tooltip("How far away the drone tries to hover from a target, horizontally")]
    private float _moveToHorizontalDistanceFromTarget = 3f;
    [SerializeField, Tooltip("How high the drone tries to hover relative to its target")]
    private float _hoverHeight = 2f;
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private FiniteStateMachine.FSMParameter _finishedTutorial;

    private Transform _player;

    private void Awake()
    {
        _player = Player.Motion.PlayerMovement.Instance.transform;
    }

    public void StopVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    public void IdleMovement()
    {
        if (_drone.GetActionStateMachineInstance().GetBool(_finishedTutorial)) // not clean, DroneMovement shouldnt deal w/ fsm stuff.
        {
            float rotateDegrees = _idleSpinSpeed * Time.deltaTime;
            Vector3 newRotation = _rigidbody.rotation.eulerAngles;
            newRotation.y += rotateDegrees; // Will it confine the rotation properly?
            _rigidbody.MoveRotation(Quaternion.Euler(newRotation));
        }
        else
        {
            RotateTowardsTarget(_player);
        }
    }

    /// <summary>
    /// Moves the drone along a vector, but not past the end of that vector.
    /// </summary>
    public void MoveDrone(Vector3 toTarget)
    {
        float maxMovementDistance = Time.deltaTime * _movementSpeed;
        Vector3 positionChange = Vector3.MoveTowards(Vector3.zero, toTarget, maxMovementDistance);
        _rigidbody.velocity = positionChange / Time.deltaTime;
    }

    /// <summary>
    /// Returns a vector from the drone to a point near the target.
    /// </summary>
    public Vector3 FromDroneToNear(Transform toHoverNear, bool backAwayIfTooClose = true)
    {
        Vector3 targetPosition = MoveUpAndTowardsDrone(toHoverNear.position, transform.position, _hoverHeight
            , _moveToHorizontalDistanceFromTarget);
        Vector3 toTargetPosition = targetPosition - transform.position;

        if (!backAwayIfTooClose)
        {
            // If it's already closer horizontally than the target position, use its current horizontal position
            Vector3 toSamePosition = toHoverNear.position - transform.position;
            float horizontalDistanceToSamePosition = (new Vector2(toSamePosition.x, toSamePosition.z)).magnitude;
            if (horizontalDistanceToSamePosition < _moveToHorizontalDistanceFromTarget)
            {
                toTargetPosition.x = 0;
                toTargetPosition.z = 0;
            }
        }

        return toTargetPosition;
    }

    /// <summary>
    /// Moves a vector up and towards the drone (but not vertically towards the drone, only 
    /// horizontally.) The point is to get a vector near an object, for the drone to hover 
    /// next to.
    /// </summary>
    private static Vector3 MoveUpAndTowardsDrone(Vector3 toMove, Vector3 dronePosition
        , float upDistance, float horizontalDistance)
    {
        Vector3 horizontalDirection = toMove - dronePosition;
        horizontalDirection.y = 0;
        horizontalDirection.Normalize();

        return toMove + (upDistance * Vector3.up) - (horizontalDistance * horizontalDirection);
    }

    /// <summary>
    /// Rotates the drone towards the provided transform.
    /// </summary>
    public void RotateTowardsTarget(Transform target)
    {
        Vector3 displacement = target.position - _rigidbody.position;
        Quaternion rotateTowards = Quaternion.LookRotation(displacement);
        Quaternion nextRotation = Quaternion.RotateTowards(transform.rotation, rotateTowards, _turnSpeed * Time.deltaTime);
        _rigidbody.MoveRotation(nextRotation);
    }
}
