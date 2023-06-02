using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    //Input
    private float xInput;
    private float zInput;
    private bool jumpInput;
    private bool jumpInputStop;
    private bool grabInput;

    //Checks
    private bool isGrounded;
    private bool HoldingGrappleInput;

    private bool coyoteTime;
    private bool wallJumpCoyoteTime;
    private bool isJumping;

    private float startWallJumpCoyoteTime;

    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isGrounded = core.CollisionSenses.Ground;

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckCoyoteTime();

        xInput = player.InputHandler.NormInputX;
        zInput = player.InputHandler.NormInputZ;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputStop = player.InputHandler.JumpInputStop;
        grabInput = player.InputHandler.GrabInput;
        HoldingGrappleInput = player.InputHandler.isHoldingGrappleButton;

        CheckJumpMultiplier();

        if (isGrounded && core.Movement.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);
        }
        else if (jumpInput && player.JumpState.CanJump())
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (player.GrappleDirectionalState.CheckIfCanGrapple() && HoldingGrappleInput)
        {
            stateMachine.ChangeState(player.GrappleDirectionalState);
        }
        else
        {
           //core.Movement.CheckIfShouldFlip(xInput);
           core.Movement.SetVelocityXandZ(playerData.movementVelocity * xInput, playerData.movementVelocity * zInput);

            //player.Anim.SetFloat("yVelocity", core.Movement.CurrentVelocity.y);
            //player.Anim.SetFloat("xVelocity", Mathf.Abs(core.Movement.CurrentVelocity.x));
        }

    }

    private void CheckJumpMultiplier()
    {
        if (isJumping)
        {
            if (jumpInputStop)
            {
                core.Movement.SetVelocityY(core.Movement.CurrentVelocity.y * playerData.variableJumpHeightMultiplier);
                isJumping = false;
            }
            else if (core.Movement.CurrentVelocity.y <= 0f)
            {
                isJumping = false;
            }

        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void CheckCoyoteTime()
    {
        if (coyoteTime && Time.time > startTime + playerData.coyoteTime)
        {
            coyoteTime = false;
            player.JumpState.DecreaseAmountOfJumpsLeft();
        }
    }

    public void StartCoyoteTime() => coyoteTime = true;


    public void SetIsJumping() => isJumping = true;
}
