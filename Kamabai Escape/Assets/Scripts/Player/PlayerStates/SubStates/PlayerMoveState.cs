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
        var forward = Camera.main.transform.forward;
        var right = Camera.main.transform.right;

        var movementDirection = forward * yInput + right * xInput;
        movementDirection.Normalize();

        Debug.Log("The X Velocity total " + playerData.movementVelocity * xInput);
        core.Movement.SetVelocityXandZ(playerData.movementVelocity * movementDirection.x, playerData.movementVelocity * movementDirection.z);
        //core.Movement.SetVelocityX(playerData.movementVelocity * xInput);
        //core.Movement.SetVelocityZ(playerData.movementVelocity * yInput);


        if (!isExitingState)
        {
            if (xInput == 0 || yInput == 0)
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
