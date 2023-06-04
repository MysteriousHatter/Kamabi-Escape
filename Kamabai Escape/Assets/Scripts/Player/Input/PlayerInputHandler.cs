using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UIElements;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private InputActionReference grappleButtonReference;
    [SerializeField] private InputActionReference verticalAction;
    [SerializeField] private InputActionReference grappleDirectionalButtonReferenceH;
    [SerializeField] private InputActionReference grappleDirectionalButtonReferenceV;
    private Camera cam;

    public Vector3 RawMovementInput { get; private set; }
    public float RawGrappleDirectionInput { get; private set; }

    public float reelInandOut { get; private set; }
    public float NormInputX { get; private set; }
    public float NormInputZ { get; private set; }

    public float GrappleRotationX { get; private set; }
    public float GrappleRotationY { get; private set; }
    public bool JumpInput { get; private set; }

    public bool GrappleInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    public bool GrabInput { get; private set; }
    public bool isHoldingGrappleButton { get; private set; }
    public bool GrappleInputStop { get; set; }
    public bool qbuttonIsPressed { get; set; }
    public bool ebuttonIsPressed { get; set; }

    public bool cancelInput { get; set; }



    [SerializeField]
    private float inputHoldTime = 0.2f;
    private float jumpInputStartTime;

    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private float tapDuration = 0.1f;

    private float holdTime;



    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        grappleButtonReference.action.performed += OnGrappleInputPerformed;
        grappleButtonReference.action.canceled += OnGrappleInitiated;
        grappleDirectionalButtonReferenceH.action.performed += OnGrappleDirectionInputHorizontal;
        grappleDirectionalButtonReferenceH.action.canceled += OnGrappleDirectionInputCanceledHorizontal;
        grappleDirectionalButtonReferenceV.action.performed += OnGrappleDirectionInputVertical;
        grappleDirectionalButtonReferenceV.action.canceled += OnGrappleDirectionInputCanceledVertical;
        //grappleButtonReference.action.performed +=


        cam = Camera.main;
    }

    private void Update()
    {
        InputSystem.Update();
        Debug.Log("The current action map " + playerInput.currentActionMap);
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
        if (context.interaction is TapInteraction) { Debug.Log("We are tapping for grapple"); }
        else if (context.interaction is HoldInteraction) 
        { 
            Debug.Log("We are holding");
            isHoldingGrappleButton = true;
            GrappleInputStop = false;
        }
        //if (context.interaction is TapInteraction) { Debug.Log("We are tapping for grapple"); }
    }

    public void OnGrappleInitiated(InputAction.CallbackContext context)
    {
          Debug.Log("We are not holding");
          //isHoldingGrappleButton = false;
          GrappleInputStop = true;
         playerInput.SwitchCurrentActionMap("Grapple Gameplay");
         //Debug.Log("We are not holding " + playerInput.currentActionMap);
         verticalAction.action.performed += ReelInReelOutPerformed;
         verticalAction.action.canceled += ReelInReelOutCanceled;

    }

    public void UseGrappleInput() => GrappleInput = false;
    public void UseHoldingGrapple() => isHoldingGrappleButton = false;
    public void UseJumpInput() => JumpInput = false;
    public void SwitchActionMaps() => playerInput.SwitchCurrentActionMap("Gameplay");

    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }

    public void ReelInReelOutPerformed(InputAction.CallbackContext context)
    {

        if(context.ReadValue<float>() > 0)
        {
            if(context.control == Keyboard.current.qKey)
            {
                qbuttonIsPressed = true;
            }
            else if(context.control == Keyboard.current.eKey)
            {
                ebuttonIsPressed = true;
            }
        }
    }

    public void ReelInReelOutCanceled(InputAction.CallbackContext context)
    {
        qbuttonIsPressed = false;
        ebuttonIsPressed = false;
    }

    //private void ReelInReelOutCanceled(InputAction.CallbackContext context) => reelInandOut = 0;
    private void OnGrappleDirectionInputCanceledHorizontal(InputAction.CallbackContext context) => GrappleRotationX = 0;
    private void OnGrappleDirectionInputCanceledVertical(InputAction.CallbackContext context) => GrappleRotationY = 0;

}