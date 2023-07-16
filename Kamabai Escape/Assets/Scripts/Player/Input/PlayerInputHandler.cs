using System;
using UnityEditor.ShaderGraph;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerInput playerInput;
    private Player player;
    [SerializeField] private InputActionReference grappleButtonReference;
    [SerializeField] private InputActionReference verticalAction;
    [SerializeField] private InputActionReference grappleDirectionalButtonReferenceH;
    [SerializeField] private InputActionReference grappleDirectionalButtonReferenceV;
    [SerializeField] private InputActionReference helpButtonReference;
    private Camera cam;

    public Vector3 RawMovementInput { get; private set; }
    public float RawGrappleDirectionInput { get; private set; }

    public float reelInandOut { get; private set; }
    public float NormInputX { get; private set; }
    public float NormInputZ { get; private set; }
    public float ReelInput { get; private set; }
    public float GrappleRotationX { get; private set; }
    public float GrappleRotationY { get; private set; }
    public bool JumpInput { get; private set; }

    public bool GrappleInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    public bool GrabInput { get; private set; }
    public bool CameraInput { get; private set; }
    public bool isHoldingGrappleButton { get; private set; }
    public bool isTappingGrappleButton { get; private set; }
    public bool GrappleInputStop { get; set; }
    public bool qbuttonIsPressed { get; set; }
    public bool ebuttonIsPressed { get; set; }

    public bool cancelInput { get; set; }

    public int pressCounter { get; private set; }

    public bool isSwinging { get; set; }


    [SerializeField]
    private float inputHoldTime = 0.2f;
    private float jumpInputStartTime;
    public HelpBoard helpBoard;
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private float tapDuration = 0.1f;
    private bool readBoard = false;

    private void OnEnable()
    {
        grappleButtonReference.action.Enable();
    }

    private void OnDisable()
    {
        grappleButtonReference.action.Disable();
    }

    private void Start()
    {
        //player.playerInput = GetComponent<PlayerInput>();
        player = GetComponent<Player>();
        grappleButtonReference.action.started += OnGrappleInputPerformed;
        helpButtonReference.action.started += OnHelpButtonPressed;
        //grappleDirectionalButtonReferenceH.action.performed += OnGrappleDirectionInputHorizontal;
        //grappleDirectionalButtonReferenceH.action.canceled += OnGrappleDirectionInputCanceledHorizontal;
        //grappleDirectionalButtonReferenceV.action.performed += OnGrappleDirectionInputVertical;
        //grappleDirectionalButtonReferenceV.action.canceled += OnGrappleDirectionInputCanceledVertical;
        pressCounter = 0;
        //grappleButtonReference.action.performed +=


        cam = Camera.main;
    }

    private void Update()
    {
        InputSystem.Update();
       
        Debug.Log("The current input map" + player.playerInput.currentActionMap);
        CheckJumpInputHoldTime();
        
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();
        Debug.Log("Move with Context " + RawMovementInput);

        NormInputX = RawMovementInput.x;
        NormInputZ = RawMovementInput.y;
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpInput = true;
            JumpInputStop = false;
            jumpInputStartTime = Time.time;
        }

        if (context.canceled)
        {
            JumpInputStop = true;
        }
    }

    // Add this function to your class:
    private void OnHelpButtonPressed(InputAction.CallbackContext context)
    {
        // Check if the player is close to the help board
        if (helpBoard.isPlayerNear)
        {
            // If the player is near, open the help UI
            player.time.StopTimer();
            helpBoard.OnHelp();
            readBoard = true;

        }
    }

    public void OnCloseInput(InputAction.CallbackContext context)
    {
        if(context.started) 
        {
            if(readBoard)
            {
               helpBoard.CloseHelp();
                player.time.StartTimer();
                readBoard = false;
            }
        }
    }

    public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GrabInput = true;
        }

        if (context.canceled)
        {
            GrabInput = false;
        }
    }

    public void ChangeCameraAngle(InputAction.CallbackContext context)
    {
        if(context.started) 
        {
            CameraInput = true;
        }
    }

    public void OnEscapeInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (context.interaction is TapInteraction)
            {
                Debug.Log("We are tapping the escape button");
                pressCounter++;
            }
        }
    }

    public void OnCancelGrappleInput(InputAction.CallbackContext context)
    {
        if(context.started) 
        {
            cancelInput = true; 
        }
    }

    public void OnGrappleDirectionInputHorizontal(InputAction.CallbackContext context)
    {
        RawGrappleDirectionInput = context.ReadValue<float>();

        GrappleRotationX -= RawGrappleDirectionInput;
        //DashDirectionInput = Vector2Int.RoundToInt(RawDashDirectionInput.normalized);
    }

    public void OnGrappleDirectionInputVertical(InputAction.CallbackContext context)
    {
        RawGrappleDirectionInput = context.ReadValue<float>();

        GrappleRotationY -= RawGrappleDirectionInput;
        //DashDirectionInput = Vector2Int.RoundToInt(RawDashDirectionInput.normalized);
    }


    public void OnGrappleInputPerformed(InputAction.CallbackContext context)
    {
        switch(context.phase)
        {
            case InputActionPhase.Started:
                if (context.interaction is SlowTapInteraction)
                {
                    Debug.Log("We are Charging");
                    isHoldingGrappleButton = true;
                    isTappingGrappleButton = false;
                    GrappleInputStop = false;
                    player.inputType = Player.GrappleInputs.HoldButton;
                }
 
                break;
            case InputActionPhase.Performed:
                if (context.interaction is SlowTapInteraction)
                {
                    Debug.Log("We are show charged fire");
                    isHoldingGrappleButton = false;
                    isTappingGrappleButton = false;
                    GrappleInputStop = true;
                }
                break;
            case InputActionPhase.Canceled:
                grappleButtonReference.action.canceled += OnGrappleInitiated;
                break;
        }
        //rappleInputStop = false;
        //if (context.interaction is TapInteraction) { Debug.Log("We are tapping for grapple"); }
    }

    public void OnGrappleInitiated(InputAction.CallbackContext context)
    {
          Debug.Log("We are not holding");
          isHoldingGrappleButton = false;
          isTappingGrappleButton = false;
          GrappleInputStop = true;
         //Debug.Log("We are not holding " + playerInput.currentActionMap);
         //verticalAction.action.performed += ReelInReelOutPerformed;
         //verticalAction.action.canceled += ReelInReelOutCanceled;

    }

    public void UseGrappleInput() => GrappleInput = false;
    public void UseHoldingGrapple() => isHoldingGrappleButton = false;

    public void UseTappingGrapple() => isTappingGrappleButton = false;
    public void UseJumpInput() => JumpInput = false;
    public void UseCameraInput() => CameraInput = false;
    public void SwitchActionToGameplay() => player.playerInput.SwitchCurrentActionMap("Gameplay");
    public void SwitchActionMapToGrapple() => player.playerInput.SwitchCurrentActionMap("Grapple Gameplay");
    public void SwitchActionMapToCaptured() => player.playerInput.SwitchCurrentActionMap("Captured Gameplay");
    public void SwitchActionMapToHelp() => player.playerInput.SwitchCurrentActionMap("Help Gameplay");

    public int getCurrentPressCounter() => pressCounter;

    public void resetPressCounter() => pressCounter = 0;

    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }

    public void ReelInReelOutPerformed(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();
        Debug.Log("Move with Context " + RawMovementInput);

        ReelInput = RawMovementInput.y;
    }


    //private void ReelInReelOutCanceled(InputAction.CallbackContext context) => reelInandOut = 0;
    private void OnGrappleDirectionInputCanceledHorizontal(InputAction.CallbackContext context) => GrappleRotationX = 0;
    private void OnGrappleDirectionInputCanceledVertical(InputAction.CallbackContext context) => GrappleRotationY = 0;

}