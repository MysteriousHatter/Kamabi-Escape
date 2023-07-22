using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : CoreComponent
{
    public Rigidbody RB { get; private set; }

    public Vector3 FacingDirection { get; private set; }
    [HideInInspector] public int arrowDirection = 0;

    public bool CanSetVelocity { get; set; }

    public Vector3 CurrentVelocity { get; private set; }

    private Vector3 workspace;
    Quaternion initialRotation;
    Quaternion toRotation;


    public float rotationSpeed = 900f;

    protected override void Awake()
    {
        base.Awake();

        RB = GetComponentInParent<Rigidbody>();

        FacingDirection = RB.velocity;
        initialRotation = RB.transform.rotation;
        CanSetVelocity = true;
    }

    public void LogicUpdate()
    {
        CurrentVelocity = RB.velocity;
    }

    #region Set Functions

    public void SetVelocityZero()
    {
        workspace = Vector3.zero;
        SetFinalVelocity();
    }

    public void SetVelocity(float velocity, Vector3 direction)
    {
        workspace = direction * velocity;
        SetFinalVelocity();
    }

    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y, CurrentVelocity.z);
        SetFinalVelocity();
    }

    public void SetVelocityXandZ(float velocityX, float velocityZ)
    {
        workspace.Set(velocityX, CurrentVelocity.y, velocityZ);
        SetFinalVelocity();
        
    }
    public bool IsFacingForwards()
    {
        // Current facing direction of the player
        Vector3 facingDirection = this.FacingDirection;

        // Direction of the movement
        Vector3 movementDirection = this.CurrentVelocity.normalized;

        // Calculate the dot product
        float dotProduct = Vector3.Dot(facingDirection, movementDirection);

        // If the dot product is greater than 0, the player is facing forwards
        return dotProduct > 0f;
    }

    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity, CurrentVelocity.z);
        SetFinalVelocity();
    }

    public void SetVelocityZ(float velocity)
    {
        workspace.Set(CurrentVelocity.x, CurrentVelocity.y, velocity);
        SetFinalVelocity();
    }

    private void SetFinalVelocity()
    {
        if (CanSetVelocity)
        {
            Debug.Log("The current RB velocity" + RB.velocity);
            RB.velocity = workspace;
            CurrentVelocity = workspace;
            FacingDirection = workspace;
            Debug.Log("The current Facing Direction" + FacingDirection);
        }
    }

    public void CheckIfShouldFlip()
    {
        if (FacingDirection != Vector3.zero)
        {
            Flip();
        }
    }

    public void CheckIfShouldFlipAir()
    {
        if (FacingDirection != Vector3.zero)
        {
            FlipAir();
        }
    }

    public void Flip()
    {
        Quaternion toRotation = Quaternion.LookRotation(FacingDirection, Vector3.up);
        RB.transform.rotation = Quaternion.RotateTowards(RB.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        Vector3 movementDirection = core.Movement.FacingDirection;  // Di
        // Calculate the direction from the player to the current facing direction
        Vector3 directionToCurrentFacing = core.Movement.FacingDirection;

        // Calculate the direction to each cardinal direction (you can customize these based on your game's orientation)
        Vector3 forwardDirection = Vector3.forward;
        Vector3 backwardDirection = Vector3.back;
        Vector3 leftDirection = Vector3.left;
        Vector3 rightDirection = Vector3.right;

        // Calculate the angles between the player's current facing direction and each cardinal direction
        float angleForward = Vector3.Angle(directionToCurrentFacing, forwardDirection);
        float angleBackward = Vector3.Angle(directionToCurrentFacing, backwardDirection);
        float angleLeft = Vector3.Angle(directionToCurrentFacing, leftDirection);
        float angleRight = Vector3.Angle(directionToCurrentFacing, rightDirection);

        // Define a threshold to consider when checking the angles
        float angleThreshold = 45f;

        // Check the angles to determine the facing direction
        if (angleForward < angleThreshold)
        {
            Debug.Log("Player is facing forwards.");
            arrowDirection = 1;
        }
        else if (angleBackward < angleThreshold)
        {
            Debug.Log("Player is facing backwards.");
            arrowDirection = -1;
        }
        else if (angleLeft < angleThreshold)
        {
            Debug.Log("Player is facing left.");
            arrowDirection = 1;
        }
        else if (angleRight < angleThreshold)
        {
            Debug.Log("Player is facing right.");
            arrowDirection = 1;
        }
        else
        {
            Debug.Log("Player is facing some other direction.");
        }
    }

    public void FlipAir()
    {
        Quaternion toRotation = Quaternion.LookRotation(FacingDirection, Vector3.up);
        toRotation.eulerAngles = new Vector3(0f, toRotation.eulerAngles.y, 0f);
        RB.transform.rotation = Quaternion.RotateTowards(RB.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

    }

    #endregion
}
