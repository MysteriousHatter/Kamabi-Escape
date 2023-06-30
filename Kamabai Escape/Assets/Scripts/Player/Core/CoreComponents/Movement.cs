using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : CoreComponent
{
    public Rigidbody RB { get; private set; }

    public Vector3 FacingDirection { get; private set; }

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
    }

    public void FlipAir()
    {
        Quaternion toRotation = Quaternion.LookRotation(FacingDirection, Vector3.up);
        toRotation.eulerAngles = new Vector3(0f, toRotation.eulerAngles.y, 0f);
        RB.transform.rotation = Quaternion.RotateTowards(RB.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

    }

    #endregion
}
