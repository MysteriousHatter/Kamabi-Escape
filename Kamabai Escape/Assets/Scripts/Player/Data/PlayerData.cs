using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName ="newPlayerData", menuName ="Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float movementVelocity = 10f;
    

    [Header("Jump State")]
    public float jumpVelocity = 15f;
    public int amountOfJumps = 1;

    [Header("In Air State")]
    public float coyoteTime = 0.2f;
    public float variableJumpHeightMultiplier = 0.5f;

    [Header("Grapple State")]
    public float distance = 10f;
    public LayerMask mask;
    public float grappleSpeed = 3f;
    public float minDistance = 0.7f;
    public float maxDistance;
    public float lengthStep = 0.1f;
    public float scrollSensitivity = 1.0f;
    public float animationDuration = 3f;
    public float sensitivityY = 15F;
    public float minimumY = -360F;
    public float maximumY = 360F;
    public float sensitivityX = 15F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float extendCableSpeed = 25f;
    public float forwardThrustForce = 30f;
    public float SwingThrustForce;
    public float maxRotation = 220;
    public float grappleCooldown = 0.5f;
    public float angleOffset = 90f;
    public float swingHeight = 6f;
    public float rotationSpeed = 40f;
    public float maximumAngle = 270f;
    public float holdTimeScale = 0.25f;
    public float releaseForceMagnitude = 10f;

    [Header("Homing Grapple")]
    public float homingRadius = 5f;
    public GameObject reticlePrefab;

    [Header("Captured State")]
    public int maxNumberofPresses = 10;
    public int increaseNumberOfPresses = 5;
    public int maxNumberOfPressesPlaceholder = 10;
    public int maxNumberofPressesLimit = 40;



}
