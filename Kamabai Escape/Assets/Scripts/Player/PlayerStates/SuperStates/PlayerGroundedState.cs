using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected float xInput;
    protected float yInput;

    protected bool isTouchingCeiling;

    private bool JumpInput;
    private bool grabInput;
    private bool HoldingGrappleInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isTouchingLedge;
    private bool dashInput;

    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = core.CollisionSenses.Ground;
        isTouchingWall = core.CollisionSenses.WallFront;
        isTouchingLedge = core.CollisionSenses.LedgeHorizontal;
        isTouchingCeiling = core.CollisionSenses.Ceiling;
    }

    public override void Enter()
    {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();
        player.GrappleDirectionalState.ResetCanGrapple();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputZ;
        Debug.Log("the current y input " + yInput + "And X input " + xInput);
        JumpInput = player.InputHandler.JumpInput;
        grabInput = player.InputHandler.GrabInput;
        HoldingGrappleInput = player.InputHandler.isHoldingGrappleButton;


        if (JumpInput && player.JumpState.CanJump())
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if(player.GrappleDirectionalState.CheckIfCanGrapple() && HoldingGrappleInput)
        {
            Debug.Log("I can grapple");
            stateMachine.ChangeState(player.GrappleDirectionalState);
        }
        else if (!isGrounded)
        {
            player.InAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.InAirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
