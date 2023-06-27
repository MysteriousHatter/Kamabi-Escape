﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

public class Player : MonoBehaviour
{
    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerHomingGrapple HomingState { get; private set; }

    public PlayerCapturedState capturedState { get; private set; }
    public GameObject enemyPrefab { get; set; }

    public event Action HitKid;



    public PlayerInput playerInput { get; set; }
    public Transform GrappleDirectionIndicator { get; private set; }

    public SphereCollider MovementCollider { get; private set; }

    public PlayerGrappleState GrappleDirectionalState { get; private set; }

    [SerializeField]
    private PlayerData playerData;
    #endregion

    #region Components
    public Core Core { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody RB { get; private set; }
    public SpringJoint joint {get;  set;}
    public SpringJoint swingJoint { get; set;}

    public UnityEvent capturedEvent;
    public enum GrappleInputs
    {
        TapButton,
        HoldButton,
        NoInput
    }

    public GrappleInputs inputType { get; set; }

    #endregion

    #region Other Variables
    private Vector3 workspace;
    [SerializeField] public GameObject playerHand;
    [SerializeField] public LineRenderer line;
    [SerializeField] public LineRenderer lineShadow;
    public GameObject changeColorScale;
    public GameObject UICapturedBox;
    public Vector3 offset;
    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        Core = GetComponentInChildren<Core>();

        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
        capturedState = new PlayerCapturedState(this, StateMachine, playerData, "captured");
        GrappleDirectionalState = new PlayerGrappleState(this, StateMachine, playerData, "Grapple");
        UICapturedBox = GameObject.Find("CapturedUIBox");
        //TODO We will make this it's own method
        playerData.maxNumberofPresses = playerData.maxNumberOfPressesPlaceholder;

    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody>();
        UICapturedBox.SetActive(false);
        //joint = GetComponent<ConfigurableJoint>();
        GrappleDirectionIndicator = transform.Find("GrappleDirectionIndicator");
        MovementCollider = GetComponent<SphereCollider>();
        inputType = GrappleInputs.NoInput;
        StateMachine.Initialize(IdleState);
        if(changeColorScale == null)
        {
            changeColorScale = GameObject.FindGameObjectWithTag("MainCamera");
        }
        if(line == null) 
        {
            line = LineRenderer.FindObjectOfType<LineRenderer>();
        }
        if(capturedEvent == null) { capturedEvent = new UnityEvent(); }

        capturedEvent.AddListener(PlayerIsCaptured);

        //line.SetPosition(0, playerHand.transform.position);
        //line.SetPosition(1, playerHand.transform.position);
    }

    private void Update()
    {
        Core.LogicUpdate();
        StateMachine.CurrentState.LogicUpdate();
        UICapturedBox.transform.rotation = Quaternion.identity;
        Debug.Log("the current state machine " + StateMachine.CurrentState);
        Debug.Log("The current action map " + playerInput.currentActionMap.name);
    }

    private void LateUpdate()
    {
        UICapturedBox.transform.position = this.transform.position + offset;
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Other Functions
    public void SetColliderHeight(float height)
    {
        Vector3 center = MovementCollider.center;
        workspace.Set(MovementCollider.bounds.size.x, height, MovementCollider.bounds.size.z);

        workspace.Set(1, height, 1);

        transform.localScale = workspace;
    }

    public void OnDestroy()
    {
        if (joint.IsDestroyed()) { Destroy(joint.GetComponent<SpringJoint>()); }
    }

    public void PlayerIsCaptured()
    {
       StateMachine.ChangeState(capturedState);
       UICapturedBox.SetActive(true);
    }

    public void PlayerDeath()
    {
        UICapturedBox.SetActive(false);
        this.gameObject.SetActive(false);
    }
    #endregion
}