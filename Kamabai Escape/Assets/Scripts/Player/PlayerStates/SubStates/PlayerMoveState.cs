using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
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

        core.Movement.CheckIfShouldFlip();

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 cameraUp = Camera.main.transform.up;

        Vector3 movementDirection = (cameraForward * yInput) + (cameraRight * xInput);
        movementDirection.Normalize();
        Vector3 horizontalMovement = Vector3.ProjectOnPlane(movementDirection, cameraUp).normalized;
        Vector3 verticalMovement = Vector3.ProjectOnPlane(movementDirection, cameraRight).normalized;

        Vector3 finalMovementDirection = horizontalMovement + verticalMovement;
        finalMovementDirection.Normalize();

        Debug.Log("The X Velocity total " + playerData.movementVelocity * finalMovementDirection.x);
        core.Movement.SetVelocityXandZ(playerData.movementVelocity  * finalMovementDirection.x, playerData.movementVelocity * finalMovementDirection.z);
        //core.Movement.SetVelocityX(playerData.movementVelocity * xInput);
        //core.Movement.SetVelocityZ(playerData.movementVelocity * yInput);


        if (!isExitingState)
        {
            if (xInput == 0 && yInput == 0)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
