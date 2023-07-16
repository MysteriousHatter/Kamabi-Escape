using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPoint;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float respawnTime;
    [SerializeField]
    private GameObject deathUI;

    private float respawnTimeStart;

    private bool respawn;

    //private CinemachineVirtualCamera CVC;

    private void Start()
    {
        //CVC = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        CheckRespawn();
        EnableDeathUI();
    }

    private void EnableDeathUI()
    {
        if(!player.active)
        {
            deathUI.SetActive(true);
        }
        else
        {
            deathUI.SetActive(false);
        }
    }

    public void Respawn()
    {
        respawnTimeStart = Time.time;
        respawn = true;
    }

    private void CheckRespawn()
    {
        if(Time.time >= respawnTimeStart + respawnTime && respawn)
        {
            var playerTemp = Instantiate(player, respawnPoint);
           // CVC.m_Follow = playerTemp.transform;
            respawn = false;
        }
    }
}
