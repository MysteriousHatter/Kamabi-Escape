using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
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

    #endregion

    #region Other Variables
    private Vector3 workspace;
    [SerializeField] public GameObject playerHand;
    [SerializeField] public LineRenderer line;
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
        GrappleDirectionalState = new PlayerGrappleState(this, StateMachine, playerData, "Grapple");
        
    }

    private void Start()
    {
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody>();
        //joint = GetComponent<ConfigurableJoint>();
        GrappleDirectionIndicator = transform.Find("GrappleDirectionIndicator");
        MovementCollider = GetComponent<SphereCollider>();
        StateMachine.Initialize(IdleState);
        //line.SetPosition(0, playerHand.transform.position);
        //line.SetPosition(1, playerHand.transform.position);
    }

    private void Update()
    {
        Core.LogicUpdate();
        StateMachine.CurrentState.LogicUpdate();
        Debug.Log("the current state machine " + StateMachine.CurrentState);
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
    #endregion
}