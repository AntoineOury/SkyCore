using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Settings for drone behaviour.
/// </summary>
[System.Serializable] // to show in inspector
public class DroneBehaviourSettings
{
    [field: Header("Drone Movement")]
    [field: SerializeField, Tooltip("How fast the drone moves.")]
    public float MovementSpeed { get; private set; } = 3f;
    [field: SerializeField, Tooltip("How fast the drone rotates towards something, in degrees/sec")]
    public float RotationSpeed { get; private set; } = 300f;
    [field: SerializeField, Tooltip("How fast the drone spins while idling next to the player, in degrees/sec")]
    public float IdleRotationSpeed { get; private set; } = 30f;
    [field: SerializeField, Tooltip("How far away the drone tries to hover from a target, horizontally")]
    public float MoveToHorizontalDistanceFromTarget { get; private set; } = 3f;
    [field: SerializeField, Tooltip("How high the drone tries to hover relative to its target")]
    public float HoverHeight { get; private set; } = 2f;

    //[field: Header("Scanning")]
    //[field: SerializeField, Tooltip("Max distance from the drone to scan things")]
    //public float DetectionRange { get; private set; } = 10f;
    //[field: SerializeField, Tooltip("Max distance from the player things can be and still will be scanned")]
    //public float MaxDistanceFromPlayerToScan { get; private set; } = 10f;

    [field: Header("Misc")]
    [field: SerializeField]
    public bool SkipTutorial { get; private set; }
}