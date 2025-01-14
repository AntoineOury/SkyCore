using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPathfinding;


public class DroneMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _pathfindingAgentMinRadius = .48f;
    [SerializeField]
    private float _pathfindingAgentMaxRadius = .88f;
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

    [Header("Pathfinding Visualization")]
    [SerializeField]
    private bool _visualizePathfinding;
    [SerializeField]
    private GameObject _pathNodeVisual;
    [SerializeField]
    private GameObject _closedNodeVisual;
    [SerializeField]
    private GameObject _obstructedNodeVisual;

    private Transform _player;
    private AStarPathfinding _pathfinding;


    private void Awake()
    {
        _player = Player.Motion.PlayerMovement.Instance.transform;
        _pathfinding = new AStarPathfinding(gameObject.layer, _pathfindingAgentMinRadius, _pathfindingAgentMaxRadius
            , _visualizePathfinding, _pathNodeVisual, _closedNodeVisual, _obstructedNodeVisual);
    }

    public void StopVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    public void IdleMovement()
    {
        float rotateDegrees = _idleSpinSpeed * Time.deltaTime;
        Vector3 newRotation = _rigidbody.rotation.eulerAngles;
        newRotation.y += rotateDegrees;
        _rigidbody.MoveRotation(Quaternion.Euler(newRotation));
    }

    /// <summary>
    /// Moves the drone along a vector, but not past the end of that vector.
    /// (Not instantly. The physics tick must finish first. Also, there could be an obstacle.)
    /// Should be called from a FixedUpdate method until the drone reaches his destination.
    /// </summary>
    /// <returns>True if the drone will arrive at its destination once this physics tick completes, or false otherwise.</returns>
    public bool MoveDrone(Vector3 targetPosition)
    {
        Vector3 toTarget = _pathfinding.CalcPathAndGetVectorToSomewhereOnIt(_rigidbody.position, targetPosition);

        Vector3 positionChange = Vector3.MoveTowards(Vector3.zero, toTarget, Time.deltaTime * _movementSpeed);
        _rigidbody.velocity = positionChange / Time.deltaTime;
        return positionChange == toTarget && toTarget == targetPosition - _rigidbody.position;
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
    /// This function moves the drone to a point that is a specified distance directly in front of the player.
    /// It should be called in an Update() method until the drone has arrived.
    /// </summary>
    /// <param name="distanceFromPlayer">How far in front of the player the drone will stop.</param>
    /// <param name="keepFacingPlayer">If set to true, the drone will constantly keep rotating to face the player as it moves toward him.</param>
    /// <param name="height">(Optional) The drone will try to stay at this height above the player. This parameter's value is defaulted so that the drone is a bit above the player, making it look down at you at an angle.</param>
    /// <returns>True if the drone has arrived at its destination, or false otherwise.</returns>
    public bool MoveDroneInFrontOfPlayer(float distanceFromPlayer, bool keepFacingPlayer, float height = 3f)
    {
        Vector3 targetPosition = _player.transform.position + (_player.transform.forward * distanceFromPlayer);
        targetPosition.y = _player.transform.position.y + height;

        bool arrived = MoveDrone(targetPosition);

        if (keepFacingPlayer && !arrived) // dunno if the 2nd part of the && is necessary 
        {
            RotateTowardsTarget(_player.transform.position);
        }

        return arrived;
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
    /// (Not instantly. The physics tick must finish first. Also, there could be an obstacle.)
    /// </summary>
    /// <param name="target">The transform to look at.</param>
    public void RotateTowardsTarget(Transform target)
    {
        RotateTowardsTarget(target.position);
    }

    /// <summary>
    /// Rotates the drone towards the specified point in world space. 
    /// (Not instantly. The physics tick must finish first. Also, there could be an obstacle.)
    /// </summary>
    /// <param name="target">The point to look at.</param>
    public void RotateTowardsTarget(Vector3 target)
    {
        Vector3 displacement = target - _rigidbody.position;
        Quaternion rotateTowards = Quaternion.LookRotation(displacement);
        Quaternion nextRotation = Quaternion.RotateTowards(transform.rotation, rotateTowards, _turnSpeed * Time.deltaTime);
        _rigidbody.MoveRotation(nextRotation);
    }
}
