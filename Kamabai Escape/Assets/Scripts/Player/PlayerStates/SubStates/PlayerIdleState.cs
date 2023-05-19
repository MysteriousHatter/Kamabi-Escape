using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        //core.Movement.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if ((xInput != 0 || yInput != 0))
            {
                Debug.Log("We are moving on the XInput " + xInput + ", " + yInput);
                stateMachine.ChangeState(player.MoveState);
            }
        }       
        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
