using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCapturedState : PlayerAbilityState
{
    private GameObject enemy;

    private int totalTapsToEscape = 0;

    public PlayerCapturedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy = player.enemyPrefab;
        player.InputHandler.SwitchActionMapToCaptured();

    }

    public override void LogicUpdate()
    {
        totalTapsToEscape = player.InputHandler.getCurrentPressCounter();
        if(totalTapsToEscape >= playerData.maxNumberofPresses) 
        {
            Debug.Log("We are free");
            isAbilityDone = true;
            enemy.GetComponent<Guard>().isStunned = true;
            player.transform.SetParent(null);
            player.GetComponent<Rigidbody>().isKinematic = false;
            player.GetComponent<Collider>().enabled = true;
            player.InputHandler.resetPressCounter();
            player.UICapturedBox.SetActive(false);
            stateMachine.ChangeState(player.JumpState);
            player.InputHandler.SwitchActionToGameplay();
            playerData.maxNumberofPresses += playerData.increaseNumberOfPresses;
            if(playerData.maxNumberofPresses >= playerData.maxNumberofPressesLimit)
            {
                playerData.maxNumberofPresses = playerData.maxNumberofPressesLimit;
            }


        }
    }

    public override void Exit() 
    {

        base.Exit();
    }
}
